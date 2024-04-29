using System.Net;

namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http result
/// </summary>
public class HttpResult<T>
{
    /// <summary>
    /// create instance of type HttpResult
    /// </summary>
    public HttpResult()
    {
        Message = string.Empty;
        Data = default!;
        Headers = new();
        Cookies = new();
    }

    /// <summary>
    /// create instance of type HttpResult
    /// </summary>
    /// <param name="isSuccess">is success</param>
    /// <param name="code">status code</param>
    /// <param name="message">message</param>
    /// <param name="data">data</param>
    /// <param name="headers">headers</param>
    /// <param name="cookies">cookies</param>
    /// <param name="retryCount">retry count</param>
    /// <param name="timeConsuming">time consuming</param>
    public HttpResult(bool isSuccess, HttpStatusCode code, string message, T data, Dictionary<string, string> headers, List<Cookie> cookies, int retryCount, TimeSpan timeConsuming)
    {
        IsSuccess = isSuccess;
        Code = code;
        Message = message;
        Data = data;
        Headers = headers;
        RetryCount = retryCount;
        TimeConsuming = timeConsuming;
        Cookies = cookies;
    }

    /// <summary>
    /// indicate request is success
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// http response status code
    /// </summary>
    public HttpStatusCode Code { get; set; }

    /// <summary>
    /// response message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// response text
    /// </summary>
    public string? StringContent { get; set; }

    /// <summary>
    /// data
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// response headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; }

    /// <summary>
    /// retry count
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// time consuming
    /// </summary>
    public TimeSpan TimeConsuming { get; set; }

    /// <summary>
    /// cookie
    /// </summary>
    public List<Cookie> Cookies { get; set; }
}
