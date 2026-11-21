using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ClassIsland.YRAND.Services;

namespace ClassIsland.YRAND.Views;

/// <summary>
/// v2.0.0.19：更新提示改为独立可关闭窗口（不再走 ClassIsland 通知流）。
/// 展示新版本号与一句话说明，附发布台直达链接；用户可点「以后再说」关闭，
/// 关闭即不打扰（同一版本不再重复弹出）。
/// </summary>
public class UpdatePromptWindow : Window
{
	/// <summary>国内版本发布台（分享码已附带，打开即用）。</summary>
	public const string ReleaseSiteUrl = "https://ade236ccde4e195c4.app.workbuddy.host/?sharecode=UV5IFW4S3E";

	private readonly UpdateCheckService _update;

	public UpdatePromptWindow(UpdateCheckService update)
	{
		_update = update;
		Title = "YRANDOM 更新";
		Width = 480.0;
		MinHeight = 220.0;
		SizeToContent = SizeToContent.WidthAndHeight;
		CanResize = false;
		ShowInTaskbar = false;
		Topmost = true;
		WindowStartupLocation = WindowStartupLocation.CenterScreen;

		TextBlock title = new TextBlock
		{
			Text = $"发现新版本 v{update.LatestVersion}",
			FontSize = 20.0,
			FontWeight = FontWeight.Bold,
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		TextBlock current = new TextBlock
		{
			Text = $"当前版本 v{FormatVersion(update.Current)} · 检查自国内发布台",
			FontSize = 12.5,
			Foreground = (IBrush)new SolidColorBrush(Color.FromRgb(138, 138, 152)),
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		};
		TextBlock notes = new TextBlock
		{
			Text = string.IsNullOrWhiteSpace(update.LatestNotes) ? "本次更新内容详见发布台修改说明。" : update.LatestNotes,
			TextWrapping = TextWrapping.Wrap,
			FontSize = 13.5,
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		};
		TextBlock hint = new TextBlock
		{
			Text = "发布台提供全部历史版本与完整修改说明，下载 .cipx 后在「ClassIsland → 插件管理 → 本地安装」中安装（建议先卸载旧版）。",
			TextWrapping = TextWrapping.Wrap,
			FontSize = 12.0,
			Foreground = (IBrush)new SolidColorBrush(Color.FromRgb(138, 138, 152)),
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};

		Button goSite = new Button
		{
			Content = "前往发布台",
			Padding = new Thickness(18.0, 8.0, 18.0, 8.0)
		};
		goSite.Click += delegate
		{
			OpenUrl(ReleaseSiteUrl);
		};

		Button later = new Button
		{
			Content = "以后再说",
			Padding = new Thickness(18.0, 8.0, 18.0, 8.0)
		};
		later.Click += delegate
		{
			Close();
		};

		StackPanel btns = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 10.0
		};
		btns.Children.Add((Control)(object)goSite);
		if (!string.IsNullOrWhiteSpace(update.DownloadUrl))
		{
			Button dl = new Button
			{
				Content = "直接下载 .cipx",
				Padding = new Thickness(18.0, 8.0, 18.0, 8.0)
			};
			dl.Click += delegate
			{
				OpenUrl(update.DownloadUrl);
				Close();
			};
			btns.Children.Add((Control)(object)dl);
		}
		btns.Children.Add((Control)(object)later);

		ScrollViewer doc = new ScrollViewer
		{
			MaxHeight = 300.0,
			Content = (Control)(object)new StackPanel
			{
				Margin = new Thickness(24.0),
				Children =
				{
					(Control)(object)title,
					(Control)(object)current,
					(Control)(object)notes,
					(Control)(object)hint,
					(Control)(object)btns
				}
			}
		};
		Content = doc;
	}

	private static string FormatVersion(Version v)
	{
		string text = v.ToString();
		while (text.Split('.').Length > 3 && text.EndsWith(".0"))
		{
			text = text.Substring(0, text.Length - 2);
		}
		return text;
	}

	private static void OpenUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			return;
		}
		try
		{
			Process.Start(new ProcessStartInfo(url)
			{
				UseShellExecute = true
			});
		}
		catch (Exception)
		{
		}
	}
}
