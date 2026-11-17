using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Shared;
using ClassIsland.YRAND.Models;
using ClassIsland.YRAND.Services;
using CompiledAvaloniaXaml;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassIsland.YRAND.Views.SettingsPages;

[ClassIsland.Core.Attributes.SettingsPageInfo("ima.yrand", "YRAND", "\uEAFC", "\uEAFB", ClassIsland.Core.Enums.SettingsWindow.SettingsPageCategory.External)]
public class YrandSettingsPage : SettingsPageBase, INotifyPropertyChanged
{
	private readonly YrandHostService? _service;

	[CompilerGenerated]
	private static Action<object> _0021XamlIlPopulateOverride;

	public YrandSettings Settings => _service?.Settings ?? new YrandSettings();

	public string NameListText
	{
		get
		{
			return Settings.NameList;
		}
		set
		{
			if (!(Settings.NameList == value))
			{
				Settings.NameList = value;
				Save("NameListText", "RosterSummary");
			}
		}
	}

	public string RosterSummary
	{
		get
		{
			int count = YrandRosterService.Parse(Settings.NameList, Settings.MergeDuplicateNames).Count;
			string value = (Settings.MergeDuplicateNames ? "，同名已合并" : "");
			return $"共 {count} 人可参与抽取{value}。";
		}
	}

	public bool MergeDuplicateNames
	{
		get
		{
			return Settings.MergeDuplicateNames;
		}
		set
		{
			Settings.MergeDuplicateNames = value;
			Save("MergeDuplicateNames", "RosterSummary");
		}
	}

	public bool NonRepeating
	{
		get
		{
			return Settings.NonRepeating;
		}
		set
		{
			Settings.NonRepeating = value;
			Save("NonRepeating");
		}
	}

	public double SpinSeconds
	{
		get
		{
			return Settings.SpinSeconds;
		}
		set
		{
			if (!(Math.Abs(Settings.SpinSeconds - value) < 0.01))
			{
				Settings.SpinSeconds = value;
				Save("SpinSeconds", "SpinSecondsText");
			}
		}
	}

	public string SpinSecondsText => $"{Settings.SpinSeconds:0.#} 秒";

	public double ContinueChance
	{
		get
		{
			return Settings.ContinueChance;
		}
		set
		{
			int num = (int)Math.Round(value);
			if (Settings.ContinueChance != num)
			{
				Settings.ContinueChance = num;
				Save("ContinueChance", "ContinueChanceText");
			}
		}
	}

	public string ContinueChanceText => $"{Settings.ContinueChance}%";

	public bool MicBoostEnabled
	{
		get
		{
			return Settings.MicBoostEnabled;
		}
		set
		{
			if (Settings.MicBoostEnabled != value)
			{
				Settings.MicBoostEnabled = value;
				Save("MicBoostEnabled");
			}
		}
	}

	public bool MicBonusRandomized
	{
		get
		{
			return Settings.MicBonusRandomized;
		}
		set
		{
			if (Settings.MicBonusRandomized != value)
			{
				Settings.MicBonusRandomized = value;
				Save("MicBonusRandomized", "MicBonusModeText");
			}
		}
	}

	public string MicBonusModeText => Settings.MicBonusRandomized ? "随声浪（随机）" : "固定";

	public double MicFixedBonus
	{
		get
		{
			return Settings.MicFixedBonus;
		}
		set
		{
			int num = (int)Math.Round(value);
			if (Settings.MicFixedBonus != (double)num)
			{
				Settings.MicFixedBonus = num;
				Save("MicFixedBonus", "MicFixedBonusText");
			}
		}
	}

	public string MicFixedBonusText => $"+{Settings.MicFixedBonus:0}%";

	public double MicMaxBonus
	{
		get
		{
			return Settings.MicMaxBonus;
		}
		set
		{
			int num2 = (int)Math.Round(value);
			if (Settings.MicMaxBonus != (double)num2)
			{
				Settings.MicMaxBonus = num2;
				Save("MicMaxBonus", "MicMaxBonusText");
			}
		}
	}

	public string MicMaxBonusText => $"+{Settings.MicMaxBonus:0}%";

	public double MaxContinuousCount
	{
		get
		{
			return Settings.MaxContinuousCount;
		}
		set
		{
			int num = (int)Math.Round(value);
			if (Settings.MaxContinuousCount != num)
			{
				Settings.MaxContinuousCount = num;
				Save("MaxContinuousCount", "MaxContinuousCountText");
			}
		}
	}

	public string MaxContinuousCountText => $"{Settings.MaxContinuousCount} 次";

	public double MinGapSeconds
	{
		get
		{
			return Settings.MinGapSeconds;
		}
		set
		{
			if (!(Math.Abs(Settings.MinGapSeconds - value) < 0.01))
			{
				Settings.MinGapSeconds = value;
				Save("MinGapSeconds", "GapSecondsText");
			}
		}
	}

	public double MaxGapSeconds
	{
		get
		{
			return Settings.MaxGapSeconds;
		}
		set
		{
			if (!(Math.Abs(Settings.MaxGapSeconds - value) < 0.01))
			{
				Settings.MaxGapSeconds = value;
				Save("MaxGapSeconds", "GapSecondsText");
			}
		}
	}

	public string GapSecondsText => $"{Settings.MinGapSeconds:0.#} ~ {Settings.MaxGapSeconds:0.#} 秒";

	public bool ShowNotification
	{
		get
		{
			return Settings.ShowNotification;
		}
		set
		{
			Settings.ShowNotification = value;
			Save("ShowNotification");
		}
	}

	public bool SpeechEnabled
	{
		get
		{
			return Settings.SpeechEnabled;
		}
		set
		{
			Settings.SpeechEnabled = value;
			Save("SpeechEnabled");
		}
	}

	public string SpeechTemplateText
	{
		get
		{
			return Settings.SpeechTemplate;
		}
		set
		{
			if (!(Settings.SpeechTemplate == value))
			{
				Settings.SpeechTemplate = value;
				Save("SpeechTemplateText");
			}
		}
	}

	public bool ShowWindowOnStartup
	{
		get
		{
			return Settings.ShowWindowOnStartup;
		}
		set
		{
			Settings.ShowWindowOnStartup = value;
			Save("ShowWindowOnStartup");
		}
	}

	// ---- v2.0.0.16 更新检查与匿名统计 ----

	public bool UpdateCheckEnabled
	{
		get
		{
			return Settings.UpdateCheckEnabled;
		}
		set
		{
			Settings.UpdateCheckEnabled = value;
			Save("UpdateCheckEnabled");
		}
	}

	public bool UpdateNotifyEnabled
	{
		get
		{
			return Settings.UpdateNotifyEnabled;
		}
		set
		{
			Settings.UpdateNotifyEnabled = value;
			Save("UpdateNotifyEnabled");
		}
	}

	public bool StatsEnabled
	{
		get
		{
			return Settings.StatsEnabled;
		}
		set
		{
			Settings.StatsEnabled = value;
			Save("StatsEnabled");
		}
	}

	private bool _checkingUpdate;

	public string UpdateStatusText
	{
		get
		{
			UpdateCheckService update = _service?.Update;
			if (update == null)
			{
				return "当前版本未知。";
			}
			if (_checkingUpdate)
			{
				return "正在检查更新…";
			}
			string current = update.Current.ToString();
			if (string.IsNullOrEmpty(update.LatestVersion))
			{
				return $"当前版本 v{current} · 尚未获取到最新版本信息（发布台暂不可达或未开启检查）。";
			}
			string state = (update.LatestVersion == current) ? "已是最新" : ((Version.TryParse(update.LatestVersion, out Version latest) && latest > update.Current) ? "有新版本可更新" : "发布台版本较旧");
			string extra = ((update.LatestVersion != current && !string.IsNullOrWhiteSpace(update.LatestNotes))) ? $" · {update.LatestNotes}" : "";
			return $"当前版本 v{current} · 最新版本 v{update.LatestVersion}（{state}）{extra}";
		}
	}

	private string _statsText = "统计查询中…";

	public string StatsText
	{
		get
		{
			return _statsText;
		}
		private set
		{
			_statsText = value;
			Raise("StatsText");
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public YrandSettingsPage()
	{
		_service = IAppHost.Host?.Services.GetServices<IHostedService>().OfType<YrandHostService>().FirstOrDefault();
		((StyledElement)this).DataContext = this;
		InitializeComponent();
		RefreshStatsText();
	}

	private void RefreshStatsText()
	{
		UsageStatsService stats = _service?.Stats;
		if (stats == null || !_service.Settings.StatsEnabled)
		{
			StatsText = "匿名统计已关闭，本机不再上报。";
			return;
		}
		_ = Task.Run(async delegate
		{
			long? total = await stats.QueryTotalAsync().ConfigureAwait(false);
			string text = total.HasValue ? $"全球累计活跃 {total} 次（每台设备每天最多计 1 次，无任何个人信息）" : "统计服务暂不可达（不影响插件任何功能）。";
			Dispatcher.UIThread.Post(delegate
			{
				StatsText = text;
			}, DispatcherPriority.Background);
		});
	}

	private async void OnCheckUpdate(object? sender, RoutedEventArgs e)
	{
		UpdateCheckService update = _service?.Update;
		if (update == null || _checkingUpdate)
		{
			return;
		}
		_checkingUpdate = true;
		Raise("UpdateStatusText");
		await update.CheckAsync(notify: false);
		_checkingUpdate = false;
		Raise("UpdateStatusText");
	}

	private void OnShowWindow(object? sender, RoutedEventArgs e)
	{
		_service?.ShowWindow();
	}

	private void OnHideWindow(object? sender, RoutedEventArgs e)
	{
		_service?.CloseWindow();
	}

	private void Save(params string[] alsoChanged)
	{
		_service?.SaveSettings();
		Raise(alsoChanged);
	}

	private void Raise(params string[] names)
	{
		foreach (string propertyName in names)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.3.17.0")]
	[ExcludeFromCodeCoverage]
	public void InitializeComponent(bool loadXaml = true)
	{
		if (loadXaml)
		{
			_0021XamlIlPopulateTrampoline(this);
		}
	}

	[CompilerGenerated]
	private static void _0021XamlIlPopulate(IServiceProvider P_0, YrandSettingsPage P_1)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0052: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0072: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00da: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0117: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_013b: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01b3: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e7: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0233: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_0284: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Expected O, but got Unknown
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		//IL_02be: Expected O, but got Unknown
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_0300: Expected O, but got Unknown
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Expected O, but got Unknown
		//IL_03b7: Expected O, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Expected O, but got Unknown
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Expected O, but got Unknown
		//IL_0409: Expected O, but got Unknown
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_047f: Expected O, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Expected O, but got Unknown
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Expected O, but got Unknown
		//IL_04d1: Expected O, but got Unknown
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Expected O, but got Unknown
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Expected O, but got Unknown
		//IL_0547: Expected O, but got Unknown
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Expected O, but got Unknown
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Expected O, but got Unknown
		//IL_0599: Expected O, but got Unknown
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Expected O, but got Unknown
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Expected O, but got Unknown
		//IL_05d9: Expected O, but got Unknown
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Expected O, but got Unknown
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Expected O, but got Unknown
		//IL_0685: Expected O, but got Unknown
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Expected O, but got Unknown
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Expected O, but got Unknown
		//IL_0717: Expected O, but got Unknown
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Expected O, but got Unknown
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Expected O, but got Unknown
		//IL_0769: Expected O, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Expected O, but got Unknown
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Expected O, but got Unknown
		//IL_07a9: Expected O, but got Unknown
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Expected O, but got Unknown
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Expected O, but got Unknown
		//IL_0855: Expected O, but got Unknown
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Expected O, but got Unknown
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Expected O, but got Unknown
		//IL_08e7: Expected O, but got Unknown
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Expected O, but got Unknown
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Expected O, but got Unknown
		//IL_0939: Expected O, but got Unknown
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Expected O, but got Unknown
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Expected O, but got Unknown
		//IL_0979: Expected O, but got Unknown
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Expected O, but got Unknown
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Expected O, but got Unknown
		//IL_0a25: Expected O, but got Unknown
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Expected O, but got Unknown
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Expected O, but got Unknown
		//IL_0ab7: Expected O, but got Unknown
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Expected O, but got Unknown
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Expected O, but got Unknown
		//IL_0b09: Expected O, but got Unknown
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Expected O, but got Unknown
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b44: Expected O, but got Unknown
		//IL_0b49: Expected O, but got Unknown
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Expected O, but got Unknown
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Expected O, but got Unknown
		//IL_0b8c: Expected O, but got Unknown
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2e: Expected O, but got Unknown
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c34: Expected O, but got Unknown
		//IL_0c39: Expected O, but got Unknown
		//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Expected O, but got Unknown
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Expected O, but got Unknown
		//IL_0c7c: Expected O, but got Unknown
		//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1d: Expected O, but got Unknown
		//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Expected O, but got Unknown
		//IL_0d28: Expected O, but got Unknown
		//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Expected O, but got Unknown
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Expected O, but got Unknown
		//IL_0dba: Expected O, but got Unknown
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e01: Expected O, but got Unknown
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Expected O, but got Unknown
		//IL_0e0c: Expected O, but got Unknown
		//IL_0e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e77: Expected O, but got Unknown
		//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7d: Expected O, but got Unknown
		//IL_0e82: Expected O, but got Unknown
		//IL_0e98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Expected O, but got Unknown
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Expected O, but got Unknown
		//IL_0ed4: Expected O, but got Unknown
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f32: Expected O, but got Unknown
		//IL_0f32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f38: Expected O, but got Unknown
		//IL_0f3d: Expected O, but got Unknown
		//IL_0f64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6c: Expected O, but got Unknown
		//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Expected O, but got Unknown
		//IL_0f77: Expected O, but got Unknown
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fae: Expected O, but got Unknown
		//IL_0fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb4: Expected O, but got Unknown
		//IL_0fb9: Expected O, but got Unknown
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_103f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1044: Unknown result type (might be due to invalid IL or missing references)
		//IL_1047: Expected O, but got Unknown
		//IL_1047: Unknown result type (might be due to invalid IL or missing references)
		//IL_104d: Expected O, but got Unknown
		//IL_1052: Expected O, but got Unknown
		//IL_1068: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_1096: Unknown result type (might be due to invalid IL or missing references)
		//IL_1099: Expected O, but got Unknown
		//IL_1099: Unknown result type (might be due to invalid IL or missing references)
		//IL_109f: Expected O, but got Unknown
		//IL_10a4: Expected O, but got Unknown
		//IL_10bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		XamlIlContext.Context<YrandSettingsPage> context = new XamlIlContext.Context<YrandSettingsPage>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FViews_002FSettingsPages_002FYrandSettingsPage_002Eaxaml.Singleton }, "avares://ClassIsland.YRAND/Views/SettingsPages/YrandSettingsPage.axaml")
		{
			RootObject = P_1,
			IntermediateRoot = P_1
		};
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		ScrollViewer val = new ScrollViewer();
		ScrollViewer val2 = val;
		((ISupportInitialize)val).BeginInit();
		((ContentControl)P_1).Content = (object)val;
		ScrollViewer val3;
		ScrollViewer obj = (val3 = val2);
		context.PushParent(val3);
		StackPanel val4 = new StackPanel();
		StackPanel val5 = val4;
		((ISupportInitialize)val4).BeginInit();
		((ContentControl)val3).Content = (object)val4;
		StackPanel val6;
		StackPanel obj2 = (val6 = val5);
		context.PushParent(val6);
		StackPanel obj3 = val6;
		((StyledElement)obj3).Classes.Add("settings-container");
		((StyledElement)obj3).Classes.Add("animated-intro");
		((Layoutable)obj3).MaxWidth = 750.0;
		obj3.Spacing = 6.0;
		Controls children = ((Panel)obj3).Children;
		TextBlock val7 = new TextBlock();
		TextBlock val8 = val7;
		((ISupportInitialize)val7).BeginInit();
		((AvaloniaList<Control>)(object)children).Add((Control)val7);
		((StyledElement)val8).Classes.Add("l-instructive");
		val8.Text = "随机点名";
		((ISupportInitialize)val8).EndInit();
		Controls children2 = ((Panel)obj3).Children;
		Separator val9 = new Separator();
		Separator val10 = val9;
		((ISupportInitialize)val9).BeginInit();
		((AvaloniaList<Control>)(object)children2).Add((Control)val9);
		((ISupportInitialize)val10).EndInit();
		Controls children3 = ((Panel)obj3).Children;
		SettingsExpander val11 = new SettingsExpander();
		SettingsExpander val12 = val11;
		((ISupportInitialize)val11).BeginInit();
		((AvaloniaList<Control>)(object)children3).Add((Control)val11);
		SettingsExpander val13;
		SettingsExpander obj4 = (val13 = val12);
		context.PushParent(val13);
		SettingsExpander obj5 = val13;
		obj5.IconSource = (IconSource)(object)new FluentIconSource("\ue8a5").ProvideValue();
		((HeaderedItemsControl)obj5).Header = "学生名单";
		StyledProperty<string> descriptionProperty = SettingsExpander.DescriptionProperty;
		ReflectionBindingExtension val14 = new ReflectionBindingExtension("RosterSummary");
		context.ProvideTargetProperty = SettingsExpander.DescriptionProperty;
		Binding obj6 = val14.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj5, (AvaloniaProperty)(object)descriptionProperty, (IBinding)(object)obj6, (object)null);
		StackPanel val15 = new StackPanel();
		StackPanel val16 = val15;
		((ISupportInitialize)val15).BeginInit();
		obj5.Footer = (object)val15;
		val16.Orientation = (Orientation)0;
		val16.Spacing = 8.0;
		Controls children4 = ((Panel)val16).Children;
		Button val17 = new Button();
		Button val18 = val17;
		((ISupportInitialize)val17).BeginInit();
		((AvaloniaList<Control>)(object)children4).Add((Control)val17);
		Button val19 = default(Button);
		((Interactive)val18).AddHandler((RoutedEvent)(object)Button.ClickEvent, (Delegate)new EventHandler<RoutedEventArgs>(context.RootObject.OnShowWindow), (RoutingStrategies)5, false);
		((ContentControl)val18).Content = "打开悬浮窗";
		((ISupportInitialize)val18).EndInit();
		Controls children5 = ((Panel)val16).Children;
		Button val20 = new Button();
		Button val21 = val20;
		((ISupportInitialize)val20).BeginInit();
		((AvaloniaList<Control>)(object)children5).Add((Control)val20);
		((Interactive)val21).AddHandler((RoutedEvent)(object)Button.ClickEvent, (Delegate)new EventHandler<RoutedEventArgs>(context.RootObject.OnHideWindow), (RoutingStrategies)5, false);
		((ContentControl)val21).Content = "关闭悬浮窗";
		((ISupportInitialize)val21).EndInit();
		((ISupportInitialize)val16).EndInit();
		ItemCollection items = ((ItemsControl)obj5).Items;
		StackPanel val22 = new StackPanel();
		StackPanel val23 = val22;
		((ISupportInitialize)val22).BeginInit();
		items.Add((object)val22);
		StackPanel obj7 = (val6 = val23);
		context.PushParent(val6);
		StackPanel obj8 = val6;
		obj8.Spacing = 6.0;
		Controls children6 = ((Panel)obj8).Children;
		TextBlock val24 = new TextBlock();
		TextBlock val25 = val24;
		((ISupportInitialize)val24).BeginInit();
		((AvaloniaList<Control>)(object)children6).Add((Control)val24);
		val25.Text = "每行一个名字。空行和以 # 开头的行会被忽略，改完即存即生效。悬浮窗：点一下抽人，按住拖动。";
		val25.TextWrapping = (TextWrapping)1;
		((Visual)val25).Opacity = 0.7;
		((ISupportInitialize)val25).EndInit();
		Controls children7 = ((Panel)obj8).Children;
		TextBox val26 = new TextBox();
		TextBox val27 = val26;
		((ISupportInitialize)val26).BeginInit();
		((AvaloniaList<Control>)(object)children7).Add((Control)val26);
		TextBox val28;
		TextBox obj9 = (val28 = val27);
		context.PushParent(val28);
		TextBox obj10 = val28;
		StyledProperty<string> textProperty = TextBox.TextProperty;
		ReflectionBindingExtension val29 = new ReflectionBindingExtension("NameListText")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = TextBox.TextProperty;
		Binding obj11 = val29.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj10, (AvaloniaProperty)(object)textProperty, (IBinding)(object)obj11, (object)null);
		obj10.AcceptsReturn = true;
		obj10.TextWrapping = (TextWrapping)0;
		((Layoutable)obj10).Height = 180.0;
		((Layoutable)obj10).MinWidth = 420.0;
		obj10.Watermark = "张三\n李四\n王五";
		context.PopParent();
		((ISupportInitialize)obj9).EndInit();
		context.PopParent();
		((ISupportInitialize)obj7).EndInit();
		context.PopParent();
		((ISupportInitialize)obj4).EndInit();
		Controls children8 = ((Panel)obj3).Children;
		SettingsExpander val30 = new SettingsExpander();
		SettingsExpander val31 = val30;
		((ISupportInitialize)val30).BeginInit();
		((AvaloniaList<Control>)(object)children8).Add((Control)val30);
		SettingsExpander obj12 = (val13 = val31);
		context.PushParent(val13);
		SettingsExpander obj13 = val13;
		obj13.IconSource = (IconSource)(object)new FluentIconSource("\ue8b9").ProvideValue();
		((HeaderedItemsControl)obj13).Header = "同名合并（反作弊）";
		obj13.Description = "名单内相同名字自动合并为一个。有人往名单里把同一个名字抄几遍想刷概率？开了这个，人人平等。";
		ToggleSwitch val32 = new ToggleSwitch();
		ToggleSwitch val33 = val32;
		((ISupportInitialize)val32).BeginInit();
		obj13.Footer = (object)val32;
		ToggleSwitch val34;
		ToggleSwitch obj14 = (val34 = val33);
		context.PushParent(val34);
		ToggleSwitch obj15 = val34;
		StyledProperty<bool?> isCheckedProperty = ToggleButton.IsCheckedProperty;
		ReflectionBindingExtension val35 = new ReflectionBindingExtension("MergeDuplicateNames")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		Binding obj16 = val35.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj15, (AvaloniaProperty)(object)isCheckedProperty, (IBinding)(object)obj16, (object)null);
		context.PopParent();
		((ISupportInitialize)obj14).EndInit();
		context.PopParent();
		((ISupportInitialize)obj12).EndInit();
		Controls children9 = ((Panel)obj3).Children;
		SettingsExpander val36 = new SettingsExpander();
		SettingsExpander val37 = val36;
		((ISupportInitialize)val36).BeginInit();
		((AvaloniaList<Control>)(object)children9).Add((Control)val36);
		SettingsExpander obj17 = (val13 = val37);
		context.PushParent(val13);
		SettingsExpander obj18 = val13;
		obj18.IconSource = (IconSource)(object)new FluentIconSource("\ue0be").ProvideValue();
		((HeaderedItemsControl)obj18).Header = "本轮内不重复";
		obj18.Description = "抽到的人本轮不再出现，抽完一轮自动开新一轮。关闭就是纯随机，只回避上一次抽到的同一个人。";
		ToggleSwitch val38 = new ToggleSwitch();
		ToggleSwitch val39 = val38;
		((ISupportInitialize)val38).BeginInit();
		obj18.Footer = (object)val38;
		ToggleSwitch obj19 = (val34 = val39);
		context.PushParent(val34);
		ToggleSwitch obj20 = val34;
		StyledProperty<bool?> isCheckedProperty2 = ToggleButton.IsCheckedProperty;
		ReflectionBindingExtension val40 = new ReflectionBindingExtension("NonRepeating")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		Binding obj21 = val40.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj20, (AvaloniaProperty)(object)isCheckedProperty2, (IBinding)(object)obj21, (object)null);
		context.PopParent();
		((ISupportInitialize)obj19).EndInit();
		context.PopParent();
		((ISupportInitialize)obj17).EndInit();
		Controls children10 = ((Panel)obj3).Children;
		SettingsExpander val41 = new SettingsExpander();
		SettingsExpander val42 = val41;
		((ISupportInitialize)val41).BeginInit();
		((AvaloniaList<Control>)(object)children10).Add((Control)val41);
		SettingsExpander obj22 = (val13 = val42);
		context.PushParent(val13);
		SettingsExpander obj23 = val13;
		obj23.IconSource = (IconSource)(object)new FluentIconSource("\ue916").ProvideValue();
		((HeaderedItemsControl)obj23).Header = "滑动时长（秒）";
		obj23.Description = "每轮滑动动画的时长，所有轮次匀速一致。滑动过程不显示滚到的名字，时间一到直接定格真正抽中的人。";
		StackPanel val43 = new StackPanel();
		StackPanel val44 = val43;
		((ISupportInitialize)val43).BeginInit();
		obj23.Footer = (object)val43;
		StackPanel obj24 = (val6 = val44);
		context.PushParent(val6);
		StackPanel obj25 = val6;
		obj25.Orientation = (Orientation)0;
		obj25.Spacing = 10.0;
		Controls children11 = ((Panel)obj25).Children;
		Slider val45 = new Slider();
		Slider val46 = val45;
		((ISupportInitialize)val45).BeginInit();
		((AvaloniaList<Control>)(object)children11).Add((Control)val45);
		Slider val47;
		Slider obj26 = (val47 = val46);
		context.PushParent(val47);
		Slider obj27 = val47;
		((Layoutable)obj27).Width = 200.0;
		((RangeBase)obj27).Minimum = 1.0;
		((RangeBase)obj27).Maximum = 10.0;
		obj27.TickFrequency = 0.5;
		obj27.IsSnapToTickEnabled = true;
		StyledProperty<double> valueProperty = RangeBase.ValueProperty;
		ReflectionBindingExtension val48 = new ReflectionBindingExtension("SpinSeconds")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = RangeBase.ValueProperty;
		Binding obj28 = val48.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj27, (AvaloniaProperty)(object)valueProperty, (IBinding)(object)obj28, (object)null);
		context.PopParent();
		((ISupportInitialize)obj26).EndInit();
		Controls children12 = ((Panel)obj25).Children;
		TextBlock val49 = new TextBlock();
		TextBlock val50 = val49;
		((ISupportInitialize)val49).BeginInit();
		((AvaloniaList<Control>)(object)children12).Add((Control)val49);
		TextBlock val51;
		TextBlock obj29 = (val51 = val50);
		context.PushParent(val51);
		TextBlock obj30 = val51;
		StyledProperty<string> textProperty2 = TextBlock.TextProperty;
		ReflectionBindingExtension val52 = new ReflectionBindingExtension("SpinSecondsText");
		context.ProvideTargetProperty = TextBlock.TextProperty;
		Binding obj31 = val52.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj30, (AvaloniaProperty)(object)textProperty2, (IBinding)(object)obj31, (object)null);
		((Layoutable)obj30).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)obj30).Width = 46.0;
		context.PopParent();
		((ISupportInitialize)obj29).EndInit();
		context.PopParent();
		((ISupportInitialize)obj24).EndInit();
		context.PopParent();
		((ISupportInitialize)obj22).EndInit();
		Controls children13 = ((Panel)obj3).Children;
		SettingsExpander val53 = new SettingsExpander();
		SettingsExpander val54 = val53;
		((ISupportInitialize)val53).BeginInit();
		((AvaloniaList<Control>)(object)children13).Add((Control)val53);
		SettingsExpander obj32 = (val13 = val54);
		context.PushParent(val13);
		SettingsExpander obj33 = val13;
		obj33.IconSource = (IconSource)(object)new FluentIconSource("\uf2ed").ProvideValue();
		((HeaderedItemsControl)obj33).Header = "滑动概率";
		obj33.Description = "你的心脏有多大。每次抽出后，按这个概率自动滑动到下一位：0 = 只抽一次，100 = 必滑下一位。";
		StackPanel val55 = new StackPanel();
		StackPanel val56 = val55;
		((ISupportInitialize)val55).BeginInit();
		obj33.Footer = (object)val55;
		StackPanel obj34 = (val6 = val56);
		context.PushParent(val6);
		StackPanel obj35 = val6;
		obj35.Orientation = (Orientation)0;
		obj35.Spacing = 10.0;
		Controls children14 = ((Panel)obj35).Children;
		Slider val57 = new Slider();
		Slider val58 = val57;
		((ISupportInitialize)val57).BeginInit();
		((AvaloniaList<Control>)(object)children14).Add((Control)val57);
		Slider obj36 = (val47 = val58);
		context.PushParent(val47);
		Slider obj37 = val47;
		((Layoutable)obj37).Width = 200.0;
		((RangeBase)obj37).Minimum = 0.0;
		((RangeBase)obj37).Maximum = 100.0;
		obj37.TickFrequency = 5.0;
		obj37.IsSnapToTickEnabled = true;
		StyledProperty<double> valueProperty2 = RangeBase.ValueProperty;
		ReflectionBindingExtension val59 = new ReflectionBindingExtension("ContinueChance")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = RangeBase.ValueProperty;
		Binding obj38 = val59.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj37, (AvaloniaProperty)(object)valueProperty2, (IBinding)(object)obj38, (object)null);
		context.PopParent();
		((ISupportInitialize)obj36).EndInit();
		Controls children15 = ((Panel)obj35).Children;
		TextBlock val60 = new TextBlock();
		TextBlock val61 = val60;
		((ISupportInitialize)val60).BeginInit();
		((AvaloniaList<Control>)(object)children15).Add((Control)val60);
		TextBlock obj39 = (val51 = val61);
		context.PushParent(val51);
		TextBlock obj40 = val51;
		StyledProperty<string> textProperty3 = TextBlock.TextProperty;
		ReflectionBindingExtension val62 = new ReflectionBindingExtension("ContinueChanceText");
		context.ProvideTargetProperty = TextBlock.TextProperty;
		Binding obj41 = val62.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj40, (AvaloniaProperty)(object)textProperty3, (IBinding)(object)obj41, (object)null);
		((Layoutable)obj40).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)obj40).Width = 46.0;
		context.PopParent();
		((ISupportInitialize)obj39).EndInit();
		context.PopParent();
		((ISupportInitialize)obj34).EndInit();
		context.PopParent();
		((ISupportInitialize)obj32).EndInit();
		Controls children16 = ((Panel)obj3).Children;
		SettingsExpander val63 = new SettingsExpander();
		SettingsExpander val64 = val63;
		((ISupportInitialize)val63).BeginInit();
		((AvaloniaList<Control>)(object)children16).Add((Control)val63);
		SettingsExpander obj42 = (val13 = val64);
		context.PushParent(val13);
		SettingsExpander obj43 = val13;
		obj43.IconSource = (IconSource)(object)new FluentIconSource("\ue71e").ProvideValue();
		((HeaderedItemsControl)obj43).Header = "滑动连续最高次数";
		obj43.Description = "一次启动里最多连续滑动几轮，到数强制停下。等待期间点一下圆钮可随时认怂。";
		StackPanel val65 = new StackPanel();
		StackPanel val66 = val65;
		((ISupportInitialize)val65).BeginInit();
		obj43.Footer = (object)val65;
		StackPanel obj44 = (val6 = val66);
		context.PushParent(val6);
		StackPanel obj45 = val6;
		obj45.Orientation = (Orientation)0;
		obj45.Spacing = 10.0;
		Controls children17 = ((Panel)obj45).Children;
		Slider val67 = new Slider();
		Slider val68 = val67;
		((ISupportInitialize)val67).BeginInit();
		((AvaloniaList<Control>)(object)children17).Add((Control)val67);
		Slider obj46 = (val47 = val68);
		context.PushParent(val47);
		Slider obj47 = val47;
		((Layoutable)obj47).Width = 200.0;
		((RangeBase)obj47).Minimum = 1.0;
		((RangeBase)obj47).Maximum = 20.0;
		obj47.TickFrequency = 1.0;
		obj47.IsSnapToTickEnabled = true;
		StyledProperty<double> valueProperty3 = RangeBase.ValueProperty;
		ReflectionBindingExtension val69 = new ReflectionBindingExtension("MaxContinuousCount")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = RangeBase.ValueProperty;
		Binding obj48 = val69.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj47, (AvaloniaProperty)(object)valueProperty3, (IBinding)(object)obj48, (object)null);
		context.PopParent();
		((ISupportInitialize)obj46).EndInit();
		Controls children18 = ((Panel)obj45).Children;
		TextBlock val70 = new TextBlock();
		TextBlock val71 = val70;
		((ISupportInitialize)val70).BeginInit();
		((AvaloniaList<Control>)(object)children18).Add((Control)val70);
		TextBlock obj49 = (val51 = val71);
		context.PushParent(val51);
		TextBlock obj50 = val51;
		StyledProperty<string> textProperty4 = TextBlock.TextProperty;
		ReflectionBindingExtension val72 = new ReflectionBindingExtension("MaxContinuousCountText");
		context.ProvideTargetProperty = TextBlock.TextProperty;
		Binding obj51 = val72.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj50, (AvaloniaProperty)(object)textProperty4, (IBinding)(object)obj51, (object)null);
		((Layoutable)obj50).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)obj50).Width = 46.0;
		context.PopParent();
		((ISupportInitialize)obj49).EndInit();
		context.PopParent();
		((ISupportInitialize)obj44).EndInit();
		context.PopParent();
		((ISupportInitialize)obj42).EndInit();
		Controls children19 = ((Panel)obj3).Children;
		SettingsExpander val73 = new SettingsExpander();
		SettingsExpander val74 = val73;
		((ISupportInitialize)val73).BeginInit();
		((AvaloniaList<Control>)(object)children19).Add((Control)val73);
		SettingsExpander obj52 = (val13 = val74);
		context.PushParent(val13);
		SettingsExpander obj53 = val13;
		obj53.IconSource = (IconSource)(object)new FluentIconSource("\ue916").ProvideValue();
		((HeaderedItemsControl)obj53).Header = "两轮停顿（秒）";
		obj53.Description = "定格之后到下一轮自动滑动之间的随机等待时间。";
		StackPanel val75 = new StackPanel();
		StackPanel val76 = val75;
		((ISupportInitialize)val75).BeginInit();
		obj53.Footer = (object)val75;
		StackPanel obj54 = (val6 = val76);
		context.PushParent(val6);
		StackPanel obj55 = val6;
		obj55.Orientation = (Orientation)0;
		obj55.Spacing = 10.0;
		Controls children20 = ((Panel)obj55).Children;
		TextBlock val77 = new TextBlock();
		TextBlock val78 = val77;
		((ISupportInitialize)val77).BeginInit();
		((AvaloniaList<Control>)(object)children20).Add((Control)val77);
		val78.Text = "最小";
		((Layoutable)val78).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)val78).Width = 34.0;
		((ISupportInitialize)val78).EndInit();
		Controls children21 = ((Panel)obj55).Children;
		Slider val79 = new Slider();
		Slider val80 = val79;
		((ISupportInitialize)val79).BeginInit();
		((AvaloniaList<Control>)(object)children21).Add((Control)val79);
		Slider obj56 = (val47 = val80);
		context.PushParent(val47);
		Slider obj57 = val47;
		((Layoutable)obj57).Width = 140.0;
		((RangeBase)obj57).Minimum = 1.0;
		((RangeBase)obj57).Maximum = 20.0;
		obj57.TickFrequency = 0.5;
		obj57.IsSnapToTickEnabled = true;
		StyledProperty<double> valueProperty4 = RangeBase.ValueProperty;
		ReflectionBindingExtension val81 = new ReflectionBindingExtension("MinGapSeconds")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = RangeBase.ValueProperty;
		Binding obj58 = val81.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj57, (AvaloniaProperty)(object)valueProperty4, (IBinding)(object)obj58, (object)null);
		context.PopParent();
		((ISupportInitialize)obj56).EndInit();
		Controls children22 = ((Panel)obj55).Children;
		TextBlock val82 = new TextBlock();
		TextBlock val83 = val82;
		((ISupportInitialize)val82).BeginInit();
		((AvaloniaList<Control>)(object)children22).Add((Control)val82);
		val83.Text = "最大";
		((Layoutable)val83).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)val83).Width = 34.0;
		((ISupportInitialize)val83).EndInit();
		Controls children23 = ((Panel)obj55).Children;
		Slider val84 = new Slider();
		Slider val85 = val84;
		((ISupportInitialize)val84).BeginInit();
		((AvaloniaList<Control>)(object)children23).Add((Control)val84);
		Slider obj59 = (val47 = val85);
		context.PushParent(val47);
		Slider obj60 = val47;
		((Layoutable)obj60).Width = 140.0;
		((RangeBase)obj60).Minimum = 1.0;
		((RangeBase)obj60).Maximum = 20.0;
		obj60.TickFrequency = 0.5;
		obj60.IsSnapToTickEnabled = true;
		StyledProperty<double> valueProperty5 = RangeBase.ValueProperty;
		ReflectionBindingExtension val86 = new ReflectionBindingExtension("MaxGapSeconds")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = RangeBase.ValueProperty;
		Binding obj61 = val86.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj60, (AvaloniaProperty)(object)valueProperty5, (IBinding)(object)obj61, (object)null);
		context.PopParent();
		((ISupportInitialize)obj59).EndInit();
		Controls children24 = ((Panel)obj55).Children;
		TextBlock val87 = new TextBlock();
		TextBlock val88 = val87;
		((ISupportInitialize)val87).BeginInit();
		((AvaloniaList<Control>)(object)children24).Add((Control)val87);
		TextBlock obj62 = (val51 = val88);
		context.PushParent(val51);
		TextBlock obj63 = val51;
		StyledProperty<string> textProperty5 = TextBlock.TextProperty;
		ReflectionBindingExtension val89 = new ReflectionBindingExtension("GapSecondsText");
		context.ProvideTargetProperty = TextBlock.TextProperty;
		Binding obj64 = val89.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj63, (AvaloniaProperty)(object)textProperty5, (IBinding)(object)obj64, (object)null);
		((Layoutable)obj63).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)obj63).Width = 70.0;
		context.PopParent();
		((ISupportInitialize)obj62).EndInit();
		context.PopParent();
		((ISupportInitialize)obj54).EndInit();
		context.PopParent();
		((ISupportInitialize)obj52).EndInit();
		// —— 麦克风声浪加成（v2.0.0.12） ——
		Controls childrenMic = ((Panel)obj3).Children;
		SettingsExpander micExpander = new SettingsExpander();
		((ISupportInitialize)micExpander).BeginInit();
		((AvaloniaList<Control>)(object)childrenMic).Add((Control)micExpander);
		micExpander.IconSource = (IconSource)(object)new FluentIconSource("\ueb90").ProvideValue();
		((HeaderedItemsControl)micExpander).Header = "麦克风声浪加成";
		micExpander.Description = "开启后实时采集教室音量：声浪越大，连滑概率越高，滚动也越快。无麦克风或拒绝授权时自动降级，不影响点名。加成区间默认 -35 dBFS 起加、-12 dBFS 加满。";
		ToggleSwitch micToggle = new ToggleSwitch();
		micExpander.Footer = (object)micToggle;
		micToggle.Bind(ToggleButton.IsCheckedProperty, new Binding("MicBoostEnabled", (BindingMode)2)
		{
			Source = context.RootObject
		});
		ItemCollection micItems = ((ItemsControl)micExpander).Items;
		StackPanel modeRow = new StackPanel();
		modeRow.Spacing = 6.0;
		micItems.Add((object)modeRow);
		TextBlock modeLabel = new TextBlock();
		modeLabel.Text = "加速率模式";
		modeRow.Children.Add((Control)modeLabel);
		StackPanel modeInner = new StackPanel();
		modeInner.Orientation = (Orientation)0;
		modeInner.Spacing = 10.0;
		modeRow.Children.Add((Control)modeInner);
		ToggleSwitch modeToggle = new ToggleSwitch();
		modeToggle.OnContent = (object)"随声浪（随机）";
		modeToggle.OffContent = (object)"固定";
		modeInner.Children.Add((Control)modeToggle);
		modeToggle.Bind(ToggleButton.IsCheckedProperty, new Binding("MicBonusRandomized", (BindingMode)2)
		{
			Source = context.RootObject
		});
		TextBlock modeHint = new TextBlock();
		modeHint.Text = "开 = 加成随教室音量实时爬升（随机）；关 = 音量一过阈值就给固定加成。";
		modeHint.TextWrapping = (TextWrapping)1;
		((Visual)modeHint).Opacity = 0.7;
		modeInner.Children.Add((Control)modeHint);
		StackPanel fixedRow = new StackPanel();
		fixedRow.Spacing = 6.0;
		micItems.Add((object)fixedRow);
		TextBlock fixedLabel = new TextBlock();
		fixedLabel.Text = "固定加速率（%）——固定模式下生效";
		fixedRow.Children.Add((Control)fixedLabel);
		StackPanel fixedInner = new StackPanel();
		fixedInner.Orientation = (Orientation)0;
		fixedInner.Spacing = 10.0;
		fixedRow.Children.Add((Control)fixedInner);
		Slider fixedSlider = new Slider();
		((Layoutable)fixedSlider).Width = 200.0;
		((RangeBase)fixedSlider).Minimum = 0.0;
		((RangeBase)fixedSlider).Maximum = 100.0;
		fixedSlider.TickFrequency = 5.0;
		fixedSlider.IsSnapToTickEnabled = true;
		fixedInner.Children.Add((Control)fixedSlider);
		fixedSlider.Bind(RangeBase.ValueProperty, new Binding("MicFixedBonus", (BindingMode)2)
		{
			Source = context.RootObject
		});
		TextBlock fixedText = new TextBlock();
		((Layoutable)fixedText).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)fixedText).Width = 52.0;
		fixedInner.Children.Add((Control)fixedText);
		fixedText.Bind(TextBlock.TextProperty, new Binding("MicFixedBonusText")
		{
			Source = context.RootObject
		});
		StackPanel maxRow = new StackPanel();
		maxRow.Spacing = 6.0;
		micItems.Add((object)maxRow);
		TextBlock maxLabel = new TextBlock();
		maxLabel.Text = "声浪最大加速率（%）——随机模式下生效";
		maxRow.Children.Add((Control)maxLabel);
		StackPanel maxInner = new StackPanel();
		maxInner.Orientation = (Orientation)0;
		maxInner.Spacing = 10.0;
		maxRow.Children.Add((Control)maxInner);
		Slider maxSlider = new Slider();
		((Layoutable)maxSlider).Width = 200.0;
		((RangeBase)maxSlider).Minimum = 0.0;
		((RangeBase)maxSlider).Maximum = 100.0;
		maxSlider.TickFrequency = 5.0;
		maxSlider.IsSnapToTickEnabled = true;
		maxInner.Children.Add((Control)maxSlider);
		maxSlider.Bind(RangeBase.ValueProperty, new Binding("MicMaxBonus", (BindingMode)2)
		{
			Source = context.RootObject
		});
		TextBlock maxText = new TextBlock();
		((Layoutable)maxText).VerticalAlignment = (VerticalAlignment)2;
		((Layoutable)maxText).Width = 52.0;
		maxInner.Children.Add((Control)maxText);
		maxText.Bind(TextBlock.TextProperty, new Binding("MicMaxBonusText")
		{
			Source = context.RootObject
		});
		((ISupportInitialize)micExpander).EndInit();
		Controls children25 = ((Panel)obj3).Children;
		SettingsExpander val90 = new SettingsExpander();
		SettingsExpander val91 = val90;
		((ISupportInitialize)val90).BeginInit();
		((AvaloniaList<Control>)(object)children25).Add((Control)val90);
		SettingsExpander obj65 = (val13 = val91);
		context.PushParent(val13);
		SettingsExpander obj66 = val13;
		obj66.IconSource = (IconSource)(object)new FluentIconSource("\ue74e").ProvideValue();
		((HeaderedItemsControl)obj66).Header = "发送 ClassIsland 提醒";
		obj66.Description = "每抽到一人，在 ClassIsland 主界面弹提醒播报结果。可在「应用设置 → 提醒 → YRAND」里单独调音效、特效。";
		ToggleSwitch val92 = new ToggleSwitch();
		ToggleSwitch val93 = val92;
		((ISupportInitialize)val92).BeginInit();
		obj66.Footer = (object)val92;
		ToggleSwitch obj67 = (val34 = val93);
		context.PushParent(val34);
		ToggleSwitch obj68 = val34;
		StyledProperty<bool?> isCheckedProperty3 = ToggleButton.IsCheckedProperty;
		ReflectionBindingExtension val94 = new ReflectionBindingExtension("ShowNotification")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		Binding obj69 = val94.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj68, (AvaloniaProperty)(object)isCheckedProperty3, (IBinding)(object)obj69, (object)null);
		context.PopParent();
		((ISupportInitialize)obj67).EndInit();
		context.PopParent();
		((ISupportInitialize)obj65).EndInit();
		Controls children26 = ((Panel)obj3).Children;
		SettingsExpander val95 = new SettingsExpander();
		SettingsExpander val96 = val95;
		((ISupportInitialize)val95).BeginInit();
		((AvaloniaList<Control>)(object)children26).Add((Control)val95);
		SettingsExpander obj70 = (val13 = val96);
		context.PushParent(val13);
		SettingsExpander obj71 = val13;
		obj71.IconSource = (IconSource)(object)new FluentIconSource("\ue767").ProvideValue();
		((HeaderedItemsControl)obj71).Header = "语音播报";
		obj71.Description = "抽中定格后用语音念出结果；滚动阶段不播报，只有最终结果会出声。";
		ToggleSwitch val97 = new ToggleSwitch();
		ToggleSwitch val98 = val97;
		((ISupportInitialize)val97).BeginInit();
		obj71.Footer = (object)val97;
		ToggleSwitch obj72 = (val34 = val98);
		context.PushParent(val34);
		ToggleSwitch obj73 = val34;
		StyledProperty<bool?> isCheckedProperty4 = ToggleButton.IsCheckedProperty;
		ReflectionBindingExtension val99 = new ReflectionBindingExtension("SpeechEnabled")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		Binding obj74 = val99.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj73, (AvaloniaProperty)(object)isCheckedProperty4, (IBinding)(object)obj74, (object)null);
		context.PopParent();
		((ISupportInitialize)obj72).EndInit();
		ItemCollection items2 = ((ItemsControl)obj71).Items;
		StackPanel val100 = new StackPanel();
		StackPanel val101 = val100;
		((ISupportInitialize)val100).BeginInit();
		items2.Add((object)val100);
		StackPanel obj75 = (val6 = val101);
		context.PushParent(val6);
		StackPanel obj76 = val6;
		obj76.Spacing = 6.0;
		Controls children27 = ((Panel)obj76).Children;
		TextBlock val102 = new TextBlock();
		TextBlock val103 = val102;
		((ISupportInitialize)val102).BeginInit();
		((AvaloniaList<Control>)(object)children27).Add((Control)val102);
		val103.Text = "播报词模板：{name} 会被替换成抽中的人。留空则只念名字。";
		val103.TextWrapping = (TextWrapping)1;
		((Visual)val103).Opacity = 0.7;
		((ISupportInitialize)val103).EndInit();
		Controls children28 = ((Panel)obj76).Children;
		TextBox val104 = new TextBox();
		TextBox val105 = val104;
		((ISupportInitialize)val104).BeginInit();
		((AvaloniaList<Control>)(object)children28).Add((Control)val104);
		TextBox obj77 = (val28 = val105);
		context.PushParent(val28);
		TextBox obj78 = val28;
		StyledProperty<string> textProperty6 = TextBox.TextProperty;
		ReflectionBindingExtension val106 = new ReflectionBindingExtension("SpeechTemplateText")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = TextBox.TextProperty;
		Binding obj79 = val106.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj78, (AvaloniaProperty)(object)textProperty6, (IBinding)(object)obj79, (object)null);
		((Layoutable)obj78).MinWidth = 420.0;
		obj78.Watermark = "请{name}来回答问题";
		context.PopParent();
		((ISupportInitialize)obj77).EndInit();
		context.PopParent();
		((ISupportInitialize)obj75).EndInit();
		context.PopParent();
		((ISupportInitialize)obj70).EndInit();
		Controls children29 = ((Panel)obj3).Children;
		SettingsExpander val107 = new SettingsExpander();
		SettingsExpander val108 = val107;
		((ISupportInitialize)val107).BeginInit();
		((AvaloniaList<Control>)(object)children29).Add((Control)val107);
		SettingsExpander obj80 = (val13 = val108);
		context.PushParent(val13);
		SettingsExpander obj81 = val13;
		obj81.IconSource = (IconSource)(object)new FluentIconSource("\ue960").ProvideValue();
		((HeaderedItemsControl)obj81).Header = "悬浮窗";
		obj81.Description = "置顶圆形小钮：点一下抽人，按住拖动位置。";
		ToggleSwitch val109 = new ToggleSwitch();
		ToggleSwitch val110 = val109;
		((ISupportInitialize)val109).BeginInit();
		obj81.Footer = (object)val109;
		ToggleSwitch obj82 = (val34 = val110);
		context.PushParent(val34);
		ToggleSwitch obj83 = val34;
		StyledProperty<bool?> isCheckedProperty5 = ToggleButton.IsCheckedProperty;
		ReflectionBindingExtension val111 = new ReflectionBindingExtension("ShowWindowOnStartup")
		{
			Mode = (BindingMode)2
		};
		context.ProvideTargetProperty = ToggleButton.IsCheckedProperty;
		Binding obj84 = val111.ProvideValue((IServiceProvider)context);
		context.ProvideTargetProperty = null;
		AvaloniaObjectExtensions.Bind((AvaloniaObject)(object)obj83, (AvaloniaProperty)(object)isCheckedProperty5, (IBinding)(object)obj84, (object)null);
		ToolTip.SetTip((Control)(object)obj83, (object)"ClassIsland 启动后自动显示悬浮窗");
		context.PopParent();
		((ISupportInitialize)obj82).EndInit();
		context.PopParent();
		((ISupportInitialize)obj80).EndInit();
		// —— 插件更新提示（v2.0.0.16） ——
		Controls childrenUpd = ((Panel)obj3).Children;
		SettingsExpander updExpander = new SettingsExpander();
		((ISupportInitialize)updExpander).BeginInit();
		((AvaloniaList<Control>)(object)childrenUpd).Add((Control)updExpander);
		updExpander.IconSource = (IconSource)(object)new FluentIconSource("\ue895").ProvideValue();
		((HeaderedItemsControl)updExpander).Header = "插件更新提示";
		updExpander.Description = "启动约 20 秒后自动检查一次更新（走国内发布台，无须翻墙）。发现新版本时仅在完全空闲状态下提醒一次——抽选进行中绝不打扰。";
		ToggleSwitch updToggle = new ToggleSwitch();
		updExpander.Footer = (object)updToggle;
		updToggle.Bind(ToggleButton.IsCheckedProperty, new Binding("UpdateCheckEnabled", (BindingMode)2)
		{
			Source = context.RootObject
		});
		ItemCollection updItems = ((ItemsControl)updExpander).Items;
		StackPanel updRow1 = new StackPanel();
		updRow1.Spacing = 8.0;
		updItems.Add((object)updRow1);
		TextBlock updStatus = new TextBlock();
		updStatus.TextWrapping = (TextWrapping)1;
		updRow1.Children.Add((Control)updStatus);
		updStatus.Bind(TextBlock.TextProperty, new Binding("UpdateStatusText")
		{
			Source = context.RootObject
		});
		StackPanel updRow2 = new StackPanel();
		updRow2.Orientation = (Orientation)0;
		updRow2.Spacing = 10.0;
		updRow1.Children.Add((Control)updRow2);
		Button checkBtn = new Button();
		((Interactive)checkBtn).AddHandler((RoutedEvent)(object)Button.ClickEvent, (Delegate)new EventHandler<RoutedEventArgs>(context.RootObject.OnCheckUpdate), (RoutingStrategies)5, false);
		((ContentControl)checkBtn).Content = "立即检查";
		updRow2.Children.Add((Control)checkBtn);
		TextBlock updHint = new TextBlock();
		updHint.Text = "更新方法：到版本发布台下载最新版 .cipx，ClassIsland → 插件管理 → 本地安装（建议先卸载旧版）。";
		updHint.TextWrapping = (TextWrapping)1;
		((Visual)updHint).Opacity = 0.7;
		((Layoutable)updHint).VerticalAlignment = (VerticalAlignment)2;
		updRow2.Children.Add((Control)updHint);
		StackPanel updRow3 = new StackPanel();
		updRow3.Spacing = 6.0;
		updItems.Add((object)updRow3);
		ToggleSwitch updNotifyToggle = new ToggleSwitch();
		updNotifyToggle.OnContent = (object)"发现新版时提醒我";
		updNotifyToggle.OffContent = (object)"只在设置页显示";
		updRow3.Children.Add((Control)updNotifyToggle);
		updNotifyToggle.Bind(ToggleButton.IsCheckedProperty, new Binding("UpdateNotifyEnabled", (BindingMode)2)
		{
			Source = context.RootObject
		});
		TextBlock updNotifyHint = new TextBlock();
		updNotifyHint.Text = "提醒只在无抽选、无其他通知占线时弹出，同一版本只提醒一次。";
		updNotifyHint.TextWrapping = (TextWrapping)1;
		((Visual)updNotifyHint).Opacity = 0.7;
		updRow3.Children.Add((Control)updNotifyHint);
		((ISupportInitialize)updExpander).EndInit();
		// —— 用户数量统计（v2.0.0.16） ——
		Controls childrenStats = ((Panel)obj3).Children;
		SettingsExpander statsExpander = new SettingsExpander();
		((ISupportInitialize)statsExpander).BeginInit();
		((AvaloniaList<Control>)(object)childrenStats).Add((Control)statsExpander);
		statsExpander.IconSource = (IconSource)(object)new FluentIconSource("\ue9d9").ProvideValue();
		((HeaderedItemsControl)statsExpander).Header = "用户数量统计（匿名）";
		statsExpander.Description = "每天首次启动向计数服务发一次匿名请求，让作者知道有多少课堂在用 YRAND。不包含名单、机器名、设备号等任何个人信息，统计端只是一个数字。";
		ToggleSwitch statsToggle = new ToggleSwitch();
		statsExpander.Footer = (object)statsToggle;
		statsToggle.Bind(ToggleButton.IsCheckedProperty, new Binding("StatsEnabled", (BindingMode)2)
		{
			Source = context.RootObject
		});
		ItemCollection statsItems = ((ItemsControl)statsExpander).Items;
		StackPanel statsRow = new StackPanel();
		statsRow.Spacing = 6.0;
		statsItems.Add((object)statsRow);
		TextBlock statsCount = new TextBlock();
		statsCount.TextWrapping = (TextWrapping)1;
		statsRow.Children.Add((Control)statsCount);
		statsCount.Bind(TextBlock.TextProperty, new Binding("StatsText")
		{
			Source = context.RootObject
		});
		TextBlock statsHint = new TextBlock();
		statsHint.Text = "关闭后本机完全不再上报；统计服务地址可在 settings.json 的 StatsEndpointUrl 修改。";
		statsHint.TextWrapping = (TextWrapping)1;
		((Visual)statsHint).Opacity = 0.7;
		statsRow.Children.Add((Control)statsHint);
		((ISupportInitialize)statsExpander).EndInit();
		context.PopParent();
		((ISupportInitialize)obj2).EndInit();
		context.PopParent();
		((ISupportInitialize)obj).EndInit();
		context.PopParent();
		((ISupportInitialize)P_1).EndInit();
		StyledElement val112;
		if ((val112 = (StyledElement)(object)((P_1 is StyledElement) ? P_1 : null)) != null)
		{
			NameScope.SetNameScope(val112, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	[CompilerGenerated]
	private static void _0021XamlIlPopulateTrampoline(YrandSettingsPage P_0)
	{
		if (_0021XamlIlPopulateOverride != null)
		{
			_0021XamlIlPopulateOverride(P_0);
		}
		else
		{
			_0021XamlIlPopulate(XamlIlRuntimeHelpers.CreateRootServiceProviderV3((IServiceProvider)null), P_0);
		}
	}
}
