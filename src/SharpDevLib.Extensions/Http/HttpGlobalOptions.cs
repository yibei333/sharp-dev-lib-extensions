namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http global options
/// </summary>
public class HttpGlobalOptions
{
    /// <summary>
    /// base url
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// time out timespan
    /// </summary>
    public TimeSpan? TimeOut { get; set; }

    /// <summary>
    /// retry count when failed
    /// </summary>
    public int? RetryCount { get; set; }
}
