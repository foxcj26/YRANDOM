using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClassIsland.YRAND.Models;

namespace ClassIsland.YRAND.Services;

/// <summary>
/// 插件更新检查（v2.0.0.16 新增）。
/// 启动约 20 秒后静默拉取发布台 latest.json 比较版本号（走国内发布台，无须翻墙）。
/// 发现新版本时仅在完全空闲（无抽选过程、无占线通知）的情况下提醒一次，同一版本不重复提醒。
/// 全程 8 秒超时、失败静默，绝不影响点名主流程。
/// </summary>
public class UpdateCheckService
{
	private static readonly HttpClient Http = CreateClient();

	private readonly YrandHostService _host;

	private readonly YrandSettings _settings;

	/// <summary>当前插件版本（程序集版本，与 manifest.yml 同步维护）。</summary>
	public Version Current { get; } = typeof(UpdateCheckService).Assembly.GetName().Version ?? new Version(1, 0, 0, 0);

	/// <summary>最近一次检查到的最新版本号（空 = 尚未成功获取）。</summary>
	public string LatestVersion { get; private set; } = "";

	/// <summary>最新版一句话说明。</summary>
	public string LatestNotes { get; private set; } = "";

	/// <summary>最新版下载地址。</summary>
	public string DownloadUrl { get; private set; } = "";

	/// <summary>最近一次成功检查的时间（null = 本会话尚未成功）。</summary>
	public DateTime? CheckedAt { get; private set; }

	private static HttpClient CreateClient()
	{
		HttpClient httpClient = new HttpClient();
		httpClient.Timeout = TimeSpan.FromSeconds(8.0);
		try
		{
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ClassIsland.YRAND/UpdateCheck");
		}
		catch (Exception)
		{
		}
		return httpClient;
	}

	public UpdateCheckService(YrandHostService host)
	{
		_host = host;
		_settings = host.Settings;
	}

	/// <summary>启动自动检查：延时 20 秒避开启动高峰，后台静默执行。</summary>
	public void Start()
	{
		if (!_settings.UpdateCheckEnabled || string.IsNullOrWhiteSpace(_settings.UpdateChannelUrl))
		{
			return;
		}
		_ = Task.Run(async delegate
		{
			await Task.Delay(TimeSpan.FromSeconds(20.0));
			await CheckAsync(notify: true).ConfigureAwait(false);
		});
	}

	/// <summary>执行一次检查（自动检查与设置页「立即检查」共用）。返回是否发现新版本。</summary>
	public async Task<bool> CheckAsync(bool notify)
	{
		string url = _settings.UpdateChannelUrl;
		if (string.IsNullOrWhiteSpace(url))
		{
			return false;
		}
		try
		{
			string json = await Http.GetStringAsync(url).ConfigureAwait(false);
			using JsonDocument doc = JsonDocument.Parse(json);
			string ver = (doc.RootElement.TryGetProperty("version", out JsonElement v) ? (v.GetString() ?? "") : "");
			string notes = (doc.RootElement.TryGetProperty("releaseNotes", out JsonElement n) ? (n.GetString() ?? "") : "");
			string dl = (doc.RootElement.TryGetProperty("downloadUrl", out JsonElement d) ? (d.GetString() ?? "") : "");
			if (!Version.TryParse(ver, out Version latest) || latest <= new Version(0, 0, 0, 0))
			{
				return false;
			}
			LatestVersion = ver;
			LatestNotes = notes;
			DownloadUrl = dl;
			CheckedAt = DateTime.Now;
			bool hasUpdate = latest > Current;
			if (hasUpdate && notify && _settings.UpdateNotifyEnabled && _settings.LastNotifiedVersion != ver)
			{
				string note = Truncate(notes, 40);
				_host.AnnounceBackground($"发现新版本 v{ver}：{note}。更新包见 YRAND 设置页说明。");
				_settings.LastNotifiedVersion = ver;
				_host.SaveSettings();
			}
			return hasUpdate;
		}
		catch (Exception)
		{
			CheckedAt = null;
			return false;
		}
	}

	private static string Truncate(string s, int len)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return "详见发布说明";
		}
		if (s.Length <= len)
		{
			return s;
		}
		return s.Substring(0, len) + "…";
	}
}
