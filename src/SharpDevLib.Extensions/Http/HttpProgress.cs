namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http progress
/// </summary>
public class HttpProgress
{
    private readonly TransferFileStatisticsModel _speed;

    internal HttpProgress(long total, long transfered, TransferFileStatisticsModel speed)
    {
        Total = total;
        Transfered = transfered;
        _speed = speed;
    }

    /// <summary>
    /// total length
    /// </summary>
    public long Total { get; }

    /// <summary>
    /// transfered bytes length
    /// </summary>
    public long Transfered { get; }

    /// <summary>
    /// progress
    /// </summary>
    public int Progress
    {
        get
        {
            if (Total <= 0) return 0;
            var percent = (int)(Transfered * 100.0 / Total);
            if (percent >= 100) return 100;
            return percent;
        }
    }

    /// <summary>
    /// progress string value
    /// </summary>
    public string ProgressString => $"{Progress}%";

    /// <summary>
    /// request speed
    /// </summary>
    public string Speed => _speed.Calc(Transfered);
}

internal class TransferFileStatisticsModel
{
    public TransferFileStatisticsModel(DateTime downloadTime)
    {
        TransferTime = downloadTime;
        TotalByteCount = 0;
        Speed = "0KB/S";
    }

    public DateTime TransferTime { get; set; }
    public double TotalByteCount { get; set; }
    public static readonly TimeSpan Peroid = TimeSpan.FromSeconds(1);
    public string Speed { get; set; }
    public string Calc(double currentByteCount)
    {
        var now = DateTime.Now;
        if ((now - TransferTime) < Peroid) return Speed;
        Speed = $"{(int)(((currentByteCount - TotalByteCount) / 1024) / Peroid.TotalSeconds)}KB/S";
        TotalByteCount = currentByteCount;
        TransferTime = now;
        return Speed;
    }
}
