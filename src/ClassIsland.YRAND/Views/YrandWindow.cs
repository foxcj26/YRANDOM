using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using ClassIsland.YRAND.Models;
using ClassIsland.YRAND.Services;

namespace ClassIsland.YRAND.Views;

public class YrandWindow : Window
{
	private enum RollPhase
	{
		Idle,
		Spinning,
		Waiting
	}

	private const double WindowSize = 64.0;

	private const double KnobSize = 40.0;

	private const double KnobCornerRadius = 8.0;

	private const double RollIntervalMs = 150.0;

	private readonly YrandHostService _host;

	private readonly YrandSettings _settings;

	private readonly DispatcherTimer _timer;

	private readonly TopmostKeeper _topmost;

	private Border _knob = new Border();

	private RollPhase _phase;

	private string[] _names = Array.Empty<string>();

	private DateTime _phaseStart;

	private double _phaseSeconds;

	private double _switchAcc;

	private double _switchInterval = 150.0;

	private string _lockedName = "";

	private int _continuousCount;

	private string _currentName = "";

	private bool _dragging;

	private bool _pointerDown;

	private PixelPoint _pressScreenPoint;

	private const double DragThreshold = 4.0;

	public YrandWindow(YrandHostService host)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Expected O, but got Unknown
		_host = host;
		_settings = host.Settings;
		((Window)this).Title = "YRAND";
		((Layoutable)this).Width = 64.0;
		((Layoutable)this).Height = 64.0;
		((Window)this).SystemDecorations = (SystemDecorations)0;
		((TemplatedControl)this).Background = null;
		((TopLevel)this).TransparencyLevelHint = (IReadOnlyList<WindowTransparencyLevel>)(object)new WindowTransparencyLevel[1] { WindowTransparencyLevel.Transparent };
		((Window)this).ShowInTaskbar = false;
		((WindowBase)this).Topmost = true;
		((Window)this).CanResize = false;
		((Window)this).ShowActivated = false;
		TextBlock child = new TextBlock
		{
			Text = "抽",
			FontSize = 18.0,
			FontWeight = (FontWeight)700,
			Foreground = (IBrush)(object)Brushes.White,
			TextAlignment = (TextAlignment)1,
			VerticalAlignment = (VerticalAlignment)2,
			HorizontalAlignment = (HorizontalAlignment)2
		};
		Border val = (_knob = new Border
		{
			Width = 40.0,
			Height = 40.0,
			CornerRadius = new CornerRadius(8.0),
			Background = (IBrush)new SolidColorBrush(Color.FromArgb((byte)232, (byte)26, (byte)26, (byte)32), 1.0),
			BorderBrush = (IBrush)new SolidColorBrush(Color.FromRgb((byte)124, (byte)77, byte.MaxValue), 0.55),
			BorderThickness = new Thickness(1.5),
			Child = (Control)(object)child
		});
		ToolTip.SetTip((Control)(object)val, (object)"YRAND\n点一下抽人；按住拖动；滑动中再点一下立即定格");
		((Visual)val).Effect = (IEffect)new DropShadowDirectionEffect
		{
			BlurRadius = 10.0,
			Direction = 270.0,
			ShadowDepth = 2.0,
			Opacity = 0.5
		};
		Panel val2 = new Panel();
		((AvaloniaList<Control>)(object)val2.Children).Add((Control)(object)val);
		Panel val3 = val2;
		((Layoutable)val3).Margin = new Thickness(12.0);
		((ContentControl)this).Content = val3;
		((InputElement)val).PointerPressed += Knob_OnPointerPressed;
		((InputElement)val).PointerMoved += Knob_OnPointerMoved;
		((InputElement)val).PointerReleased += Knob_OnPointerReleased;
		_timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(30.0)
		};
		_timer.Tick += Timer_Tick;
		_topmost = new TopmostKeeper((Window)(object)this);
		((TopLevel)this).Closed += delegate
		{
			_timer.Stop();
			_topmost.Dispose();
			CapturePosition();
			_host.SaveSettings();
		};
	}

	private void Knob_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		PointerPoint currentPoint = ((PointerEventArgs)e).GetCurrentPoint((Visual)(object)this);
		PointerPointProperties properties = currentPoint.Properties;
		if (properties.IsLeftButtonPressed)
		{
			_pointerDown = true;
			_dragging = false;
			currentPoint = ((PointerEventArgs)e).GetCurrentPoint((Visual)(object)this);
			_pressScreenPoint = ToScreenPoint(currentPoint.Position);
			((PointerEventArgs)e).Pointer.Capture((IInputElement)(object)_knob);
		}
	}

	private void Knob_OnPointerMoved(object? sender, PointerEventArgs e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (_pointerDown && e.Pointer.Captured != null)
		{
			PointerPoint currentPoint = e.GetCurrentPoint((Visual)(object)this);
			PixelPoint pressScreenPoint = ToScreenPoint(currentPoint.Position);
			int num = pressScreenPoint.X - _pressScreenPoint.X;
			int num2 = pressScreenPoint.Y - _pressScreenPoint.Y;
			if (_dragging || !((double)Math.Abs(num) < 4.0) || !((double)Math.Abs(num2) < 4.0))
			{
				_dragging = true;
				((Window)this).Position = ((Window)this).Position + new PixelPoint(num, num2);
				_pressScreenPoint = pressScreenPoint;
				ClampToScreen();
			}
		}
	}

	private void Knob_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (_pointerDown)
		{
			_pointerDown = false;
			((PointerEventArgs)e).Pointer.Capture((IInputElement)null);
			if (_dragging)
			{
				_dragging = false;
				CapturePosition();
				_host.SaveSettings();
			}
			else
			{
				OnKnobTapped();
			}
		}
	}

	private void OnKnobTapped()
	{
		switch (_phase)
		{
		case RollPhase.Idle:
			StartRolling();
			break;
		case RollPhase.Spinning:
			Determine(manualStop: true);
			break;
		case RollPhase.Waiting:
			FinishEpisode();
			break;
		}
	}

	private PixelPoint ToScreenPoint(Point logical)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Screen obj = ((WindowBase)this).Screens.ScreenFromWindow((WindowBase)(object)this);
		double num = ((obj != null) ? obj.Scaling : 1.0);
		PixelPoint position = ((Window)this).Position;
		int num2 = position.X + (int)Math.Round(logical.X * num);
		position = ((Window)this).Position;
		return new PixelPoint(num2, position.Y + (int)Math.Round(logical.Y * num));
	}

	public void CapturePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		YrandSettings settings = _settings;
		PixelPoint position = ((Window)this).Position;
		settings.WindowX = position.X;
		YrandSettings settings2 = _settings;
		position = ((Window)this).Position;
		settings2.WindowY = position.Y;
	}

	private void ClampToScreen()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		Screen val = ((WindowBase)this).Screens.ScreenFromWindow((WindowBase)(object)this) ?? ((WindowBase)this).Screens.Primary;
		if (val != null)
		{
			double num = ((val.Scaling <= 0.0) ? 1.0 : val.Scaling);
			Rect bounds = ((Visual)this).Bounds;
			int num2 = (int)Math.Ceiling(bounds.Width * num);
			bounds = ((Visual)this).Bounds;
			int num3 = (int)Math.Ceiling(bounds.Height * num);
			PixelPoint position = ((Window)this).Position;
			int x = position.X;
			PixelRect bounds2 = val.Bounds;
			int x2 = bounds2.X;
			bounds2 = val.Bounds;
			int x3 = bounds2.X;
			bounds2 = val.Bounds;
			int x4 = bounds2.X;
			bounds2 = val.Bounds;
			int num4 = Math.Clamp(x, x2, Math.Max(x3, x4 + bounds2.Width - num2));
			position = ((Window)this).Position;
			int y = position.Y;
			bounds2 = val.Bounds;
			int y2 = bounds2.Y;
			bounds2 = val.Bounds;
			int y3 = bounds2.Y;
			bounds2 = val.Bounds;
			int y4 = bounds2.Y;
			bounds2 = val.Bounds;
			int num5 = Math.Clamp(y, y2, Math.Max(y3, y4 + bounds2.Height - num3));
			((Window)this).Position = new PixelPoint(num4, num5);
		}
	}

	protected override void OnOpened(EventArgs e)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		base.OnOpened(e);
		_topmost.Attach();
		if (_settings.WindowX == int.MinValue || _settings.WindowY == int.MinValue)
		{
			Screen primary = ((WindowBase)this).Screens.Primary;
			double num = ((primary != null) ? primary.Scaling : 1.0);
			Rect bounds = ((Visual)this).Bounds;
			int num2 = (int)Math.Ceiling(bounds.Width * num);
			int num3;
			PixelRect bounds2;
			if (primary == null)
			{
				num3 = 0;
			}
			else
			{
				bounds2 = primary.Bounds;
				num3 = bounds2.X;
			}
			int num4;
			if (primary == null)
			{
				num4 = 1920;
			}
			else
			{
				bounds2 = primary.Bounds;
				num4 = bounds2.Width;
			}
			int num5 = num3 + (num4 - num2) - 20;
			int num6;
			if (primary == null)
			{
				num6 = 0;
			}
			else
			{
				bounds2 = primary.Bounds;
				num6 = bounds2.Y;
			}
			((Window)this).Position = new PixelPoint(num5, num6 + 80);
		}
		else
		{
			((Window)this).Position = new PixelPoint(_settings.WindowX, _settings.WindowY);
		}
		ClampToScreen();
	}

	private void StartRolling()
	{
		List<string> roster = YrandRosterService.Parse(_settings.NameList, _settings.MergeDuplicateNames);
		if (roster.Count == 0)
		{
			_host.AnnounceStatus("名单为空");
			return;
		}
		// 开局作废旧种子，同步账本，并锁定本过程的唯一最终结果
		_host.Fair.Abort();
		_host.Fair.SetRoster(roster);
		var (idx, locked) = _host.Fair.Reserve();
		if (idx < 0)
		{
			_host.AnnounceStatus("名单为空");
			return;
		}
		_lockedName = locked;
		_names = roster.ToArray();
		ToolTip.SetTip((Control)(object)_knob,
			$"YRAND\n点一下抽人；按住拖动；滑动中再点一下立即定格\n本轮指纹 {_host.Fair.Fingerprint} · 第 {_host.Fair.TotalDraws + 1} 次\n{_host.ChanceBadge()}");
		_continuousCount = 0;
		_switchInterval = 150.0;
		// 整个抽选过程只注册这一条通知；此后滚动/定格换名均原地更新，不再注册
		_host.BeginEpisodeNotification();
		StartSpin();
	}

	private void StartSpin()
	{
		_phaseSeconds = _host.GetSpinDuration();
		_phaseStart = DateTime.Now;
		_switchAcc = _switchInterval;
		_phase = RollPhase.Spinning;
		_timer.Start();
	}

	/// <summary>整套抽选过程结束：定格锁定结果，通知并播报（唯一一次消耗名额）。</summary>
	private void FinishEpisode()
	{
		_timer.Stop();
		_currentName = _lockedName;
		_host.Announce(_lockedName, 3.5);
		_phase = RollPhase.Idle;
	}

	/// <summary>中间定格的过渡名：同源公平分布取样（零消耗），避开锁定名以免提前剧透。</summary>
	private string PickTransitionName()
	{
		string vis = _host.Fair.PickVisual();
		if (vis == _lockedName)
		{
			vis = _host.Fair.PickVisual() ?? vis;
		}
		return vis ?? _lockedName;
	}

	private void Determine(bool manualStop)
	{
		_timer.Stop();
		if (!manualStop && _host.ShouldContinue(_continuousCount + 1))
		{
			// 第一段定格过渡名：两段式抽选的可见节奏；不播报、不新建通知条、不消耗名额
			_currentName = PickTransitionName();
			_continuousCount++;
			_phaseSeconds = _host.GetGapDuration();
			_phaseStart = DateTime.Now;
			_phase = RollPhase.Waiting;
			_timer.Start();
			_host.UpdateEpisodeName(_currentName);
		}
		else
		{
			FinishEpisode();
		}
	}

	private void AnnounceNext()
	{
		_timer.Stop();
		if (_host.ShouldContinue(_continuousCount + 1))
		{
			// 后续每一段定格新的过渡名，保持多轮连抽的可见节奏
			_currentName = PickTransitionName();
			_continuousCount++;
			_phaseSeconds = _host.GetGapDuration();
			_phaseStart = DateTime.Now;
			_phase = RollPhase.Waiting;
			_timer.Start();
			_host.UpdateEpisodeName(_currentName);
		}
		else
		{
			FinishEpisode();
		}
	}

	private void Timer_Tick(object? sender, EventArgs e)
	{
		if (_phase == RollPhase.Spinning)
		{
			_switchAcc += _timer.Interval.TotalMilliseconds;
			if (_switchAcc >= _switchInterval && _names.Length != 0)
			{
			_switchAcc = 0.0;
			// 视觉取样：同源公平分布，允许重复、零消耗；同一条提醒原地实时换名（滚动效果）
			string name = _host.Fair.PickVisual();
			if (name != null)
			{
				_currentName = name;
				_host.UpdateEpisodeName(name);
			}
			}
			// 滚动节奏 ease-out（150ms → 400ms+），麦克风声浪越大滚得越快
			double elapsed = (DateTime.Now - _phaseStart).TotalSeconds;
			double prog = Math.Clamp(elapsed / Math.Max(0.1, _phaseSeconds), 0.0, 1.0);
			double interval = 150.0 + 250.0 * prog * prog;
			if (_settings.MicBoostEnabled && _host.Mic.IsRunning)
			{
				double loud = Math.Clamp((_host.Mic.CurrentDb + 60.0) / 60.0, 0.0, 1.0);
				interval *= 1.0 - 0.35 * loud;
			}
			_switchInterval = interval;
			if ((DateTime.Now - _phaseStart).TotalSeconds >= _phaseSeconds)
			{
				Determine(manualStop: false);
			}
		}
		else if (_phase == RollPhase.Waiting && (DateTime.Now - _phaseStart).TotalSeconds >= _phaseSeconds)
		{
			AnnounceNext();
		}
	}
}
