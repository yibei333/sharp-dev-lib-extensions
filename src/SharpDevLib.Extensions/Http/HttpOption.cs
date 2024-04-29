using System.Net;

namespace SharpDevLib.Extensions.Http;

/// <summary>
/// request option
/// </summary>
public abstract class HttpOption
{
    /// <summary>
    /// instantient http option
    /// </summary>
    /// <param name="url">request url</param>
    public HttpOption(string url)
    {
        Url = url;
    }
    /// <summary>
    /// time out timespan
    /// </summary>
    public TimeSpan? TimeOut { get; set; }
    /// <summary>
    /// retry count when failed
    /// </summary>
    public int? RetryCount { get; set; }
    /// <summary>
    /// request url
    /// </summary>
    public string? Url { get; }
    /// <summary>
    /// headers
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
    /// <summary>
    /// cookies
    /// </summary>
    public List<Cookie>? Cookies { get; set; }
    /// <summary>
    /// receive progress event handler
    /// </summary>
    public Action<HttpProgress>? OnReceiveProgress { get; set; }
    /// <summary>
    /// send progress event handler
    /// </summary>
    public Action<HttpProgress>? OnSendProgress { get; set; }
}

/// <summary>
/// request option
/// </summary>
public abstract class HttpOption<TParameters> : HttpOption
{
    /// <summary>
    /// instantient http option
    /// </summary>
    /// <param name="url">request url</param>
    public HttpOption(string url) : base(url)
    {
    }

    /// <summary>
    /// instantient http option
    /// </summary>
    /// <param name="url">request url</param>
    /// <param name="parameters">request parameters</param>
    public HttpOption(string url, TParameters parameters) : this(url)
    {
        Parameters = parameters;
    }
    /// <summary>
    /// parameters
    /// </summary>
    public TParameters? Parameters { get; }
}
