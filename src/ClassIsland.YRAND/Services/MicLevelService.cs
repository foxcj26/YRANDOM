using System;
using System.Threading;
using NAudio.Wave;

namespace ClassIsland.YRAND.Services;

/// <summary>
/// 麦克风实时响度服务：采集 16kHz/16bit 单声道，按 100ms 窗计算 RMS，
/// 转换为 dBFS（0 = 满量程，-100 = 静音），指数平滑后供概率加成使用。
/// 任何失败（无设备/无权限/非 Windows）都会静默降级，不影响点名主流程。
/// </summary>
public class MicLevelService : IDisposable
{
    private WaveInEvent? _waveIn;
    private long _dbBits;
    private bool _failed;

    /// <summary>当前平滑后的响度（dBFS）。</summary>
    public double CurrentDb => BitConverter.Int64BitsToDouble(Interlocked.Read(ref _dbBits));

    /// <summary>采集是否正在运行。</summary>
    public bool IsRunning { get; private set; }

    /// <summary>设备不可用（已永久放弃，重试需重启插件）。</summary>
    public bool Failed => _failed;

    public void EnsureStarted()
    {
        if (IsRunning || _failed) return;
        try
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 16, 1),
                BufferMilliseconds = 100
            };
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += (_, _) => { IsRunning = false; };
            _waveIn.StartRecording();
            IsRunning = true;
        }
        catch
        {
            _failed = true;
            IsRunning = false;
            try { _waveIn?.Dispose(); } catch { /* ignore */ }
            _waveIn = null;
        }
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        int frames = e.BytesRecorded / 2;
        if (frames <= 0) return;
        double sum = 0;
        for (int i = 0; i + 1 < e.BytesRecorded; i += 2)
        {
            short v = (short)(e.Buffer[i] | (e.Buffer[i + 1] << 8));
            sum += (double)v * v;
        }
        double rms = Math.Sqrt(sum / frames) / 32768.0;
        double db = rms <= 1e-7 ? -100.0 : 20.0 * Math.Log10(rms);
        double smoothed = BitConverter.Int64BitsToDouble(Interlocked.Read(ref _dbBits)) * 0.65 + db * 0.35;
        Interlocked.Exchange(ref _dbBits, BitConverter.DoubleToInt64Bits(smoothed));
    }

    public void Stop()
    {
        try { if (_waveIn != null) _waveIn.StopRecording(); } catch { /* ignore */ }
        try { _waveIn?.Dispose(); } catch { /* ignore */ }
        _waveIn = null;
        IsRunning = false;
    }

    public void Dispose() => Stop();
}
