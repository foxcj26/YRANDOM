using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace ClassIsland.YRAND.Views;

public sealed class TopmostKeeper : IDisposable
{
	private readonly Window _window;

	private readonly DispatcherTimer _timer;

	/// <summary>
	/// v2.0.0.19：置顶维持抑制开关。长按/拖动悬浮窗期间置为 true，
	/// 避免每 2 秒的 Topmost 往返切换（z-order 变化）打断指针捕获导致拖不动。
	/// </summary>
	public volatile bool SuppressReassert;

	public TopmostKeeper(Window window, TimeSpan? interval = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		_window = window;
		_timer = new DispatcherTimer
		{
			Interval = (interval ?? TimeSpan.FromSeconds(2.0))
		};
		_timer.Tick += delegate
		{
			if (SuppressReassert)
			{
				return;
			}
			Reassert();
		};
	}

	public void Attach()
	{
		((WindowBase)_window).Topmost = true;
		_timer.Start();
	}

	public void Reassert()
	{
		if (!((WindowBase)_window).Topmost)
		{
			((WindowBase)_window).Topmost = true;
			return;
		}
		((WindowBase)_window).Topmost = false;
		((WindowBase)_window).Topmost = true;
	}

	public void Dispose()
	{
		_timer.Stop();
	}
}
