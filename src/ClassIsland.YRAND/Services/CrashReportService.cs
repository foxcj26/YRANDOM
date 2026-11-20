using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Threading;
using ClassIsland.YRAND.Models;
using Sentry;

namespace ClassIsland.YRAND.Services;

/// <summary>
/// Sentry 崩溃上报服务（v2.0.0.17）。
/// 在 settings.json 填入 <see cref="YrandSettings.SentryDsn"/> 并开启
/// <see cref="YrandSettings.CrashReportEnabled"/> 后生效；DSN 为空时不做任何网络行为。
/// 捕获范围：AppDomain 未处理异常、未观察的任务异常、Avalonia UI 线程异常。
/// </summary>
public class CrashReportService
{
	private readonly YrandHostService _host;

	private bool _initialized;

	/// <summary>Sentry 是否已初始化（DSN 有效且开关打开）。</summary>
	public bool IsEnabled => _initialized;

	public CrashReportService(YrandHostService host)
	{
		_host = host;
	}

	public void Start()
	{
		YrandSettings settings = _host.Settings;
		if (!settings.CrashReportEnabled || string.IsNullOrWhiteSpace(settings.SentryDsn))
		{
			return;
		}
		try
		{
			Action<Scope> configureScope = delegate(Scope scope)
			{
				scope.SetTag("plugin", "yrand");
				scope.SetTag("plugin.version", SelfVersion);
			};
			SentrySdk.Init(delegate(SentryOptions o)
			{
				o.Dsn = settings.SentryDsn.Trim();
				o.Release = "yrand@" + SelfVersion;
				o.Environment = "production";
				o.AutoSessionTracking = false;
				o.AttachStacktrace = true;
				o.SampleRate = 1.0f;
				o.TracesSampleRate = null;
				o.MaxQueueItems = 30;
				o.ShutdownTimeout = TimeSpan.FromSeconds(3);
			});
			SentrySdk.ConfigureScope(configureScope);
			_initialized = true;
		}
		catch (Exception)
		{
			// DSN 非法等情况：静默降级，绝不影响插件功能
			return;
		}
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandledException;
	}

	private static string SelfVersion
	{
		get
		{
			try
			{
				return typeof(CrashReportService).Assembly.GetName().Version?.ToString() ?? "unknown";
			}
			catch (Exception)
			{
				return "unknown";
			}
		}
	}

	private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception ex)
		{
			Capture(ex, fatal: e.IsTerminating);
		}
	}

	private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		Capture(e.Exception, fatal: false);
	}

	private static void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
	{
		Capture(e.Exception, fatal: false);
	}

	/// <summary>捕获并尽力上报（崩溃前给 3 秒 flush 窗口），任何失败静默吞掉。</summary>
	public static void Capture(Exception ex, bool fatal = false)
	{
		try
		{
			SentryEvent evt = new SentryEvent(ex)
			{
				Level = fatal ? SentryLevel.Fatal : SentryLevel.Error
			};
			SentrySdk.CaptureEvent(evt);
			_ = SentrySdk.FlushAsync(TimeSpan.FromSeconds(3));
		}
		catch (Exception)
		{
		}
	}
}
