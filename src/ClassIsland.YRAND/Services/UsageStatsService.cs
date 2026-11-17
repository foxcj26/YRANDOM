using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClassIsland.YRAND.Models;

namespace ClassIsland.YRAND.Services;

/// <summary>
/// 匿名使用统计（v2.0.0.16 新增）。
/// 每天首次启动向计数服务发一次 GET（默认 Abacus /hit/yrand/dau），
/// 意思仅仅是「今天又有一台设备开了 YRAND」——
/// 不携带学生名单、机器名、设备号、IP 归属等任何个人信息，统计端也只存一个数字。
/// 失败静默、可在设置页一键关闭；关闭后完全不再联网上报。
/// </summary>
public class UsageStatsService
{
	private const string Key = "yrand/dau";

	private static readonly HttpClient Http = CreateClient();

	private readonly YrandHostService _host;

	private readonly YrandSettings _settings;

	private static HttpClient CreateClient()
	{
		HttpClient httpClient = new HttpClient();
		httpClient.Timeout = TimeSpan.FromSeconds(6.0);
		try
		{
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ClassIsland.YRAND/Stats");
		}
		catch (Exception)
		{
		}
		return httpClient;
	}

	public UsageStatsService(YrandHostService host)
	{
		_host = host;
		_settings = host.Settings;
	}

	/// <summary>每日首次启动上报一次（同一天只报一次，失败当日不重试，避免刷量）。</summary>
	public void Start()
	{
		if (!_settings.StatsEnabled || string.IsNullOrWhiteSpace(_settings.StatsEndpointUrl))
		{
			return;
		}
		string today = DateTime.Now.ToString("yyyy-MM-dd");
		if (_settings.LastStatsReportDate == today)
		{
			return;
		}
		_settings.LastStatsReportDate = today;
		_host.SaveSettings();
		_ = Task.Run(async delegate
		{
			try
			{
				string baseUrl = _settings.StatsEndpointUrl.TrimEnd('/');
				await Http.GetStringAsync(baseUrl + "/hit/" + Key).ConfigureAwait(false);
			}
			catch (Exception)
			{
			}
		});
	}

	/// <summary>查询累计上报次数（近似「累计活跃天数」）。失败返回 null。</summary>
	public async Task<long?> QueryTotalAsync()
	{
		try
		{
			string baseUrl = (_settings.StatsEndpointUrl ?? "https://abacus.jasoncameron.dev").TrimEnd('/');
			string json = await Http.GetStringAsync(baseUrl + "/get/" + Key).ConfigureAwait(false);
			using JsonDocument doc = JsonDocument.Parse(json);
			if (doc.RootElement.TryGetProperty("value", out JsonElement v) && v.TryGetInt64(out long n))
			{
				return n;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
