using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassIsland.YRAND.Models;

public class YrandSettings
{
	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true,
		Converters = { (JsonConverter)new JsonStringEnumConverter() }
	};

	public string NameList { get; set; } = "张三\n李四\n王五\n赵六\n钱七\n孙八\n周九\n吴十";


	public bool MergeDuplicateNames { get; set; } = true;


	public bool NonRepeating { get; set; } = true;


	public double SpinSeconds { get; set; } = 2.5;


	public int ContinueChance { get; set; } = 50;


	public int MaxContinuousCount { get; set; } = 3;


	public double MinGapSeconds { get; set; } = 1.5;


	public double MaxGapSeconds { get; set; } = 4.0;


	public bool ShowNotification { get; set; } = true;


	public bool SpeechEnabled { get; set; } = true;


	public string SpeechTemplate { get; set; } = "请{name}来回答问题";


	public bool ShowWindowOnStartup { get; set; } = true;


	public int WindowX { get; set; } = int.MinValue;


	public int WindowY { get; set; } = int.MinValue;


	public List<string> DrawnThisRound { get; set; } = new List<string>();


	public string? LastPicked { get; set; }

	// ---- v2 公平算法（债务校正 + 冷落滑窗 + 可验证种子） ----

	/// <summary>公平权重 β：0 = 纯随机，越大越接近严格轮转。默认 2.5。</summary>
	public double FairnessBeta { get; set; } = 2.5;

	/// <summary>冷落窗口系数 γ：任何人连续 ⌈γ·N⌉ 次未被抽中则强制进入必抽池。默认 1.5。</summary>
	public double ColdWindowFactor { get; set; } = 1.5;

	// ---- 麦克风声浪加成 ----

	/// <summary>是否开启麦克风分贝加成（课堂喊得越响，连滑概率越高）。</summary>
	public bool MicBoostEnabled { get; set; } = false;

	/// <summary>加成起始阈值（dBFS，0=满量程）。默认 -35。</summary>
	public double MicThresholdDb { get; set; } = -35.0;

	/// <summary>加成饱和点（dBFS）。默认 -12（大声喊叫）。</summary>
	public double MicCeilingDb { get; set; } = -12.0;

	/// <summary>满声浪时的最大概率加成（百分点）。默认 45。</summary>
	public double MicMaxBonus { get; set; } = 45.0;

	/// <summary>加成模式：true = 随声浪实时爬升（随机感，0~MicMaxBonus）；false = 音量过阈值即给固定加成。默认 true。</summary>
	public bool MicBonusRandomized { get; set; } = true;

	/// <summary>固定模式加成（百分点）：音量超过阈值即给。默认 25。</summary>
	public double MicFixedBonus { get; set; } = 25.0;

	// ---- 公平账本（持久化，可验证） ----

	public List<int> PickedCounts { get; set; } = new();

	public List<int> LastPickedAt { get; set; } = new();

	public long TotalDraws { get; set; }

	/// <summary>最近一次抽选的种子指纹（SHA256 前 8 位 hex）。</summary>
	public string LastSeedFingerprint { get; set; } = "";

	/// <summary>最近一次抽选公布的种子（SHA256 全量 hex，可复算）。</summary>
	public string LastSeedHex { get; set; } = "";

	// ---- v2.0.0.16 更新检查与匿名统计 ----

	/// <summary>启动后是否自动检查更新（拉取发布台 latest.json，静默、无须翻墙）。</summary>
	public bool UpdateCheckEnabled { get; set; } = true;

	/// <summary>发现新版本时是否弹一条提醒（同一版本只提醒一次；抽选进行中绝不打扰）。</summary>
	public bool UpdateNotifyEnabled { get; set; } = true;

	/// <summary>匿名使用统计：每天首次启动上报一次，不含名单/机器名等任何个人信息。</summary>
	public bool StatsEnabled { get; set; } = true;

	/// <summary>更新清单地址（latest.json）。默认国内发布台，可自行更换。</summary>
	public string UpdateChannelUrl { get; set; } = "https://ade236ccde4e195c4.app.workbuddy.host/latest.json?sharecode=UV5IFW4S3E";

	/// <summary>匿名统计计数服务地址。默认 Abacus，可自建替换。</summary>
	public string StatsEndpointUrl { get; set; } = "https://abacus.jasoncameron.dev";

	/// <summary>最近一次提醒过的新版本号（同一版本不重复提醒）。</summary>
	public string LastNotifiedVersion { get; set; } = "";

	/// <summary>最近一次匿名上报日期（yyyy-MM-dd，每天最多一次）。</summary>
	public string LastStatsReportDate { get; set; } = "";

	// ---- v2.0.0.17 崩溃上报（Sentry） ----

	/// <summary>是否允许崩溃上报。DSN 为空时即使开启也不会有任何网络行为。</summary>
	public bool CrashReportEnabled { get; set; } = true;

	/// <summary>Sentry 项目 DSN（在 sentry.io 创建项目后获得）。留空 = 不上报。</summary>
	public string SentryDsn { get; set; } = "";

	public static YrandSettings Load(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				return JsonSerializer.Deserialize<YrandSettings>(File.ReadAllText(path), JsonOptions) ?? new YrandSettings();
			}
		}
		catch (Exception)
		{
		}
		return new YrandSettings();
	}

	public void Save(string path)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));
		}
		catch (Exception)
		{
		}
	}
}
