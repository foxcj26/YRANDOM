using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Abstractions.Services.SpeechService;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using ClassIsland.YRAND.Models;
using ClassIsland.YRAND.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassIsland.YRAND.Services;

public class YrandHostService : IHostedService
{
	private readonly string _settingsPath;

	private YrandSettings _settings = new YrandSettings();

	private YrandWindow? _window;

	private NotificationRequest? _lastRequest;

	private YrandNotificationControl? _maskControl;

	// 抽选过程通知：整个过程只注册一次，换名只改控件属性，绝不重复注册
	private NotificationRequest? _episodeRequest;

	private YrandNotificationControl? _episodeControl;

	private YrandNotificationProvider? _provider;

	private ISpeechService? _speech;

	private FairDrawService? _fair;

	private MicLevelService _mic = new MicLevelService();

	private UpdateCheckService? _update;

	private UsageStatsService? _stats;

	/// <summary>公平抽取核心（债务校正 + 冷落兜底 + 可验证种子）。</summary>
	public FairDrawService Fair => _fair;

	/// <summary>麦克风实时响度服务。</summary>
	public MicLevelService Mic => _mic;

	/// <summary>更新检查服务（v2.0.0.16）。</summary>
	public UpdateCheckService Update => _update ??= new UpdateCheckService(this);

	/// <summary>匿名使用统计服务（v2.0.0.16）。</summary>
	public UsageStatsService Stats => _stats ??= new UsageStatsService(this);

	public YrandSettings Settings => _settings;

	public YrandHostService(string pluginConfigFolder)
	{
		_settingsPath = Path.Combine(pluginConfigFolder, "settings.json");
	}

	public void SaveSettings()
	{
		_window?.CapturePosition();
		_settings.Save(_settingsPath);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_settings = YrandSettings.Load(_settingsPath);
		_fair = new FairDrawService(_settings);
		Update.Start();
		Stats.Start();
		if (_settings.ShowWindowOnStartup)
		{
			Dispatcher.UIThread.Post((Action)ShowWindow, DispatcherPriority.Background);
		}
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Dispatcher.UIThread.Post((Action)delegate
		{
			YrandWindow? window = _window;
			if (window != null)
			{
				((Window)window).Close();
			}
			_window = null;
		}, default(DispatcherPriority));
		_mic.Dispose();
		_settings.Save(_settingsPath);
		return Task.CompletedTask;
	}

	public void ShowWindow()
	{
		if (_window != null)
		{
			((WindowBase)_window).Show();
			((WindowBase)_window).Activate();
			return;
		}
		_window = new YrandWindow(this);
		((TopLevel)_window).Closed += delegate
		{
			_window = null;
		};
		((WindowBase)_window).Show();
	}

	public void CloseWindow()
	{
		YrandWindow? window = _window;
		if (window != null)
		{
			((Window)window).Close();
		}
	}

	public string? RollName()
	{
		List<string> list = YrandRosterService.Parse(_settings.NameList, _settings.MergeDuplicateNames);
		if (list.Count == 0)
		{
			return null;
		}
		string text = YrandRosterService.Pick(_settings, list);
		if (text == null)
		{
			return null;
		}
		SaveSettings();
		return text;
	}

	public void Announce(string name, double displaySeconds)
	{
		_fair?.Commit();
		EndEpisodeNotification();
		ShowYrand("抽选结果", name, forceNew: true, speech: true, displaySeconds, null);
	}

	/// <summary>
	/// 抽选过程开始：注册唯一一条过程通知（整个滚动+定格期间复用）。
	/// 已注册则跳过，绝不重复注册。
	/// </summary>
	public void BeginEpisodeNotification()
	{
		if (_provider == null)
		{
			_provider = IAppHost.Host?.Services.GetServices<IHostedService>().OfType<YrandNotificationProvider>().FirstOrDefault();
		}
		if (_provider == null || _episodeRequest != null)
		{
			return;
		}
		_episodeControl = new YrandNotificationControl();
		_episodeControl.Model.Prefix = "抽取中";
		_episodeControl.Model.Name = "";
		NotificationContent val = new NotificationContent((object)_episodeControl)
		{
			IsSpeechEnabled = false,
			EndTime = SessionDeadline()
		};
		_episodeRequest = new NotificationRequest
		{
			MaskContent = val
		};
		_episodeRequest.CompletedToken.Register(delegate
		{
			_episodeRequest = null;
			_episodeControl = null;
		});
		((NotificationProviderBase)_provider).ShowNotification(_episodeRequest);
	}

	/// <summary>
	/// 抽选过程中换名：只更新同一条通知的控件属性；通知已结束则静默跳过，不重新注册。
	/// </summary>
	public void UpdateEpisodeName(string name)
	{
		if (_episodeControl == null || string.IsNullOrWhiteSpace(name))
		{
			return;
		}
		_episodeControl.Model.Prefix = "抽取中";
		_episodeControl.Model.Name = name;
	}

	/// <summary>抽选过程结束：关闭过程通知（后续由「抽选结果」新条接管）。</summary>
	public void EndEpisodeNotification()
	{
		NotificationRequest? request = _episodeRequest;
		_episodeRequest = null;
		_episodeControl = null;
		request?.Cancel();
	}

	public void AnnounceStatus(string text, double displaySeconds = 3.0)
	{
		EndEpisodeNotification();
		ShowYrand("YRAND", text, forceNew: true, speech: false, displaySeconds, null);
	}

	/// <summary>
	/// 后台提示（v2.0.0.16：更新提醒等）。调度到 UI 线程后，
	/// 仅在「无抽选过程通知且无占线通知」的完全空闲状态下才发出——
	/// 抽选进行中或结果通知还挂着时直接放弃本次提示，绝不打扰点名。
	/// </summary>
	public void AnnounceBackground(string text, double displaySeconds = 7.0)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		Dispatcher.UIThread.Post(delegate
		{
			if (_episodeRequest != null || _lastRequest != null)
			{
				return;
			}
			ShowYrand("YRAND", text, forceNew: true, speech: false, displaySeconds, null);
		}, DispatcherPriority.Background);
	}

	private DateTime SessionDeadline()
	{
		double value = GetSpinDuration() + (double)Math.Max(1, _settings.MaxContinuousCount) * (Math.Clamp(_settings.MaxGapSeconds, 1.0, 60.0) + GetSpinDuration()) + 8.0;
		return DateTime.Now + TimeSpan.FromSeconds(value);
	}

	private void ShowYrand(string prefix, string name, bool forceNew, bool speech, double? durationSeconds, DateTime? endTime)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(name))
		{
			return;
		}
		if (_provider == null)
		{
			_provider = IAppHost.Host?.Services.GetServices<IHostedService>().OfType<YrandNotificationProvider>().FirstOrDefault();
		}
		if (_provider == null)
		{
			return;
		}
		if (!forceNew && _maskControl != null && _lastRequest != null)
		{
			_maskControl.Model.Prefix = prefix;
			_maskControl.Model.Name = name;
			if (speech)
			{
				Speak(name);
			}
			return;
		}
		NotificationRequest? lastRequest = _lastRequest;
		if (lastRequest != null)
		{
			lastRequest.Cancel();
		}
		_maskControl = new YrandNotificationControl();
		_maskControl.Model.Prefix = prefix;
		_maskControl.Model.Name = name;
		NotificationContent val = new NotificationContent((object)_maskControl)
		{
			IsSpeechEnabled = false
		};
		if (endTime.HasValue)
		{
			DateTime valueOrDefault = endTime.GetValueOrDefault();
			val.EndTime = valueOrDefault;
		}
		else
		{
			val.Duration = TimeSpan.FromSeconds(Math.Clamp(durationSeconds.GetValueOrDefault(3.0), 0.4, 30.0));
		}
		_lastRequest = new NotificationRequest
		{
			MaskContent = val
		};
		_lastRequest.CompletedToken.Register(delegate
		{
			_maskControl = null;
			_lastRequest = null;
		});
		((NotificationProviderBase)_provider).ShowNotification(_lastRequest);
		if (speech)
		{
			Speak(name);
		}
	}

	private void Speak(string name)
	{
		if (!_settings.SpeechEnabled)
		{
			return;
		}
		if (_speech == null)
		{
			_speech = IAppHost.Host?.Services.GetService<ISpeechService>();
		}
		if (_speech != null)
		{
			string text = (string.IsNullOrWhiteSpace(_settings.SpeechTemplate) ? name : _settings.SpeechTemplate.Replace("{name}", name));
			if (!string.IsNullOrWhiteSpace(text))
			{
				_speech.EnqueueSpeechQueue(text);
			}
		}
	}

	public double GetSpinDuration()
	{
		return Math.Clamp(_settings.SpinSeconds, 0.5, 120.0);
	}

	public double GetGapDuration()
	{
		double num = Math.Clamp(_settings.MinGapSeconds, 0.5, 60.0);
		double num2 = Math.Clamp(_settings.MaxGapSeconds, num, 60.0);
		return num + (double)RandomNumberGenerator.GetInt32(0, 1000) / 1000.0 * (num2 - num);
	}

	public bool ShouldContinue(int continuousCount)
	{
		if (continuousCount >= Math.Max(1, _settings.MaxContinuousCount))
		{
			return false;
		}
		double chance = EffectiveContinueChance();
		if (chance <= 0.0)
		{
			return false;
		}
		return RandomNumberGenerator.GetInt32(0, 10000) < (int)Math.Round(chance * 100.0);
	}

	/// <summary>
	/// 实时连滑概率：基础「滑动概率」 + 麦克风声浪加成。
	/// 分贝超过阈值后按线性爬升，至饱和点拿到全额加成（默认最高 +45%）。
	/// 最终中签者与此概率无关——概率只影响表演时长。
	/// </summary>
	public double EffectiveContinueChance()
	{
		double p = Math.Clamp(_settings.ContinueChance, 0, 100);
		if (_settings.MicBoostEnabled)
		{
			_mic.EnsureStarted();
			if (_mic.IsRunning)
			{
				double lo = _settings.MicThresholdDb;
				double hi = Math.Max(lo + 1.0, _settings.MicCeilingDb);
				if (_settings.MicBonusRandomized)
				{
					double f = Math.Clamp((_mic.CurrentDb - lo) / (hi - lo), 0.0, 1.0);
					p = Math.Min(100.0, p + Math.Clamp(_settings.MicMaxBonus, 0.0, 100.0) * f);
				}
				else if (_mic.CurrentDb >= lo)
				{
					p = Math.Min(100.0, p + Math.Clamp(_settings.MicFixedBonus, 0.0, 100.0));
				}
			}
		}
		return p;
	}

	/// <summary>当前实时连滑概率（供悬浮窗 tooltip 展示）。</summary>
	public string ChanceBadge() =>
		$"连滑概率 {EffectiveContinueChance():F0}%"
		+ (_settings.MicBoostEnabled && _mic.IsRunning ? $" · {_mic.CurrentDb:F0} dBFS" : "");
}
