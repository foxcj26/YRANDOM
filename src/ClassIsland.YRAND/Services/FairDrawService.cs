using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ClassIsland.YRAND.Models;

namespace ClassIsland.YRAND.Services;

/// <summary>
/// 公平抽取核心（v2 算法）：
///  - 债务校正：due_i = t/N − c_i，Gumbel 加权抽样 score_i = β·due_i + Gumbel(U_i)
///  - 冷落兜底：任何人连续 W = ⌈γ·N⌉ 次未被抽中则进入必抽池（硬保证）
///  - 可验证：每次抽选过程开局生成种子 S，公示指纹 SHA256(S) 前 8 位；
///    过程提交后公布 S，可用同算法复算（本文件即参考实现）
///  - 一次抽选过程只消耗一个名额：Reserve 锁定结果，仅 Commit 时推进账本
/// </summary>
public class FairDrawService
{
    private readonly YrandSettings _settings;

    private List<string> _names = new();
    private int[] _counts = Array.Empty<int>();
    private int[] _lastAt = Array.Empty<int>();
    private long _t;

    private byte[] _seed = Array.Empty<byte>();
    private ulong _drbgCounter;
    private int _reserved = -1;
    private string _fingerprint = "";

    public FairDrawService(YrandSettings settings)
    {
        _settings = settings;
        // 恢复持久化账本（按名单位置对齐，首个 SetRoster 时继承）
        _t = settings.TotalDraws;
        _counts = settings.PickedCounts?.ToArray() ?? Array.Empty<int>();
        _lastAt = settings.LastPickedAt?.ToArray() ?? Array.Empty<int>();
    }

    /// <summary>当前过程指纹（公开显示用），无过程时为空。</summary>
    public string Fingerprint => _fingerprint;

    /// <summary>全局已提交抽取次数。</summary>
    public long TotalDraws => _t;

    /// <summary>本次过程锁定的名单索引（-1 = 未锁定）。</summary>
    public int ReservedIndex => _reserved;

    // ---------------- DRBG：HMAC-SHA256 计数器流 ----------------

    private double NextUnit()
    {
        if (_seed == null || _seed.Length == 0)
        {
            _seed = RandomNumberGenerator.GetBytes(32);
            _drbgCounter = 0;
            _fingerprint = FingerprintOf(_seed);
        }
        using var h = new HMACSHA256(_seed);
        var block = h.ComputeHash(BitConverter.GetBytes(_drbgCounter++));
        ulong u = 0;
        for (int i = 0; i < 8; i++) u = (u << 8) | block[i];
        return (u >> 11) * (1.0 / 9007199254740992.0); // [0,1)
    }

    private int NextInt(int maxExclusive) =>
        Math.Max(0, Math.Min(maxExclusive - 1, (int)(NextUnit() * maxExclusive)));

    private static double Gumbel(double u)
    {
        u = Math.Clamp(u, 1e-12, 1.0 - 1e-12);
        return -Math.Log(-Math.Log(u));
    }

    public static string FingerprintOf(byte[] seed)
    {
        var h = SHA256.HashData(seed);
        var sb = new StringBuilder(8);
        for (int i = 0; i < 4; i++) sb.Append(h[i].ToString("X2"));
        return sb.ToString();
    }

    // ---------------- 名册同步 ----------------

    /// <summary>
    /// 同步名单：老成员继承账本，新成员按现有平均水位入场（不偏不倚），删除成员移除。
    /// </summary>
    public void SetRoster(IReadOnlyList<string> names)
    {
        var uniq = names.Distinct(StringComparer.Ordinal).ToList();
        int n = uniq.Count;
        var newCounts = new int[n];
        var newLastAt = new int[n];

        var oldIdx = new Dictionary<string, int>(_names.Count);
        for (int i = 0; i < _names.Count; i++) oldIdx[_names[i]] = i;

        // 已抽过成员的平均水位，用作新成员入场点
        double avg = 0; int c = 0;
        for (int i = 0; i < _names.Count && i < _lastAt.Length; i++)
        {
            if (_lastAt[i] >= 0) { avg += _lastAt[i]; c++; }
        }
        int entry = c > 0 ? (int)(avg / c) : -1;

        for (int j = 0; j < n; j++)
        {
            if (oldIdx.TryGetValue(uniq[j], out int oi) && oi < _counts.Length && oi < _lastAt.Length)
            {
                newCounts[j] = _counts[oi];
                newLastAt[j] = _lastAt[oi];
            }
            else if (j < _counts.Length && j < _lastAt.Length)
            {
                // 首次加载：按位置继承持久化账本
                newCounts[j] = _counts[j];
                newLastAt[j] = _lastAt[j];
            }
            else
            {
                newCounts[j] = 0;
                newLastAt[j] = entry;
            }
        }

        _names = uniq;
        _counts = newCounts;
        _lastAt = newLastAt;
        Persist();
    }

    // ---------------- 抽取过程 ----------------

    /// <summary>
    /// 开局锁定本过程的唯一最终结果（不推进账本）。
    /// 返回 (索引, 名字)；名单为空时返回 (-1, null)。
    /// </summary>
    public (int Index, string Name) Reserve()
    {
        int n = _names.Count;
        if (n == 0) return (-1, null);

        // 幂等：过程内重复 Reserve 永远返回同一锁定结果
        if (_reserved >= 0 && _reserved < n) return (_reserved, _names[_reserved]);

        if (_seed == null || _seed.Length == 0)
        {
            _seed = RandomNumberGenerator.GetBytes(32);
            _drbgCounter = 0;
            _fingerprint = FingerprintOf(_seed);
        }

        int idx;
        if (n == 1)
        {
            idx = 0;
        }
        else
        {
            double beta = Math.Clamp(_settings.FairnessBeta, 0.0, 50.0);
            bool windowOn = _settings.NonRepeating;
            int window = Math.Max(2, (int)Math.Ceiling(Math.Max(1.0, _settings.ColdWindowFactor) * n));

            List<int> must = null;
            if (windowOn)
            {
                for (int i = 0; i < n; i++)
                {
                    if (_t - _lastAt[i] >= window) (must ??= new List<int>()).Add(i);
                }
            }

            if (must != null && must.Count > 0)
            {
                idx = must[NextInt(must.Count)]; // 冷落兜底：必抽池内随机
            }
            else
            {
                int best = 0;
                double bestScore = double.NegativeInfinity;
                for (int i = 0; i < n; i++)
                {
                    double due = (double)_t / n - _counts[i];
                    double s = beta * due + Gumbel(NextUnit());
                    if (s > bestScore) { bestScore = s; best = i; }
                }
                idx = best;
            }
        }

        _reserved = idx;
        return (idx, _names[idx]);
    }

    /// <summary>
    /// 提交锁定结果（仅在最终通知发出时调用一次）：推进账本、公布种子、持久化。
    /// 返回种子十六进制（供「可验证」展示），无锁定时返回 null。
    /// </summary>
    public string Commit()
    {
        if (_reserved < 0) return null;
        int i = _reserved;
        if (i < _counts.Length) _counts[i]++;
        if (i < _lastAt.Length) _lastAt[i] = (int)_t;
        _t++;
        _reserved = -1;

        // 公布原始种子 S（指纹 = SHA256(S) 前置 8 位），任何人可用本算法复算整局
        string seedHex = (_seed != null && _seed.Length > 0) ? Convert.ToHexString(_seed) : "";
        _settings.LastSeedFingerprint = _fingerprint;
        _settings.LastSeedHex = seedHex;
        Persist();

        _seed = Array.Empty<byte>();
        _drbgCounter = 0;
        _fingerprint = "";
        return seedHex;
    }

    /// <summary>放弃本次过程（名单为空/异常），名额不消耗、种子作废。</summary>
    public void Abort()
    {
        _reserved = -1;
        _seed = Array.Empty<byte>();
        _drbgCounter = 0;
        _fingerprint = "";
    }

    /// <summary>
    /// 视觉滚动取样：同一 score 分布上抽样（温度放大），允许重复、零消耗。
    /// 让「常被看见」与「真被抽中」的机会一致。
    /// </summary>
    public string PickVisual()
    {
        int n = _names.Count;
        if (n == 0) return null;
        double beta = Math.Clamp(_settings.FairnessBeta, 0.0, 50.0) * 0.5;
        int best = 0;
        double bestScore = double.NegativeInfinity;
        for (int i = 0; i < n; i++)
        {
            double due = (double)_t / n - _counts[i];
            double s = beta * due + 2.0 * Gumbel(NextUnit());
            if (s > bestScore) { bestScore = s; best = i; }
        }
        return _names[best];
    }

    private void Persist()
    {
        _settings.PickedCounts = _counts?.ToList() ?? new List<int>();
        _settings.LastPickedAt = _lastAt?.ToList() ?? new List<int>();
        _settings.TotalDraws = _t;
    }
}
