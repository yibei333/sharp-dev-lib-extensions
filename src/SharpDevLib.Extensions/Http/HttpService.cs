using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharpDevLib.Extensions.Model;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;

namespace SharpDevLib.Extensions.Http;

internal class HttpService : IHttpService
{
    private readonly HttpGlobalOptions? _globalOptions;
    private readonly ILogger<HttpService>? _logger;

    public HttpService(IServiceProvider serviceProvider)
    {
        _globalOptions = serviceProvider.GetService<IOptions<HttpGlobalOptions>>()?.Value;
        _logger = serviceProvider.GetService<ILogger<HttpService>>();
    }

    public async Task<HttpResult<T>> GetAsync<T>(ParameterOption option, CancellationToken? cancellationToken = null)
    {
        using var client = CreateClient(option);
        var url = BuildUrl(option);
        url = BuildGetUrl(url, option.Parameters);
        if (typeof(T) == typeof(Stream)) throw new Exception($"Call GetStream Method Instead");
        _logger?.LogInformation("start http get request:{url}", url);
        return await Retry<T>(option, () => client.GetAsync(url, cancellationToken ?? new CancellationToken()));
    }

    public async Task<Stream> GetStreamAsync(ParameterOption option)
    {
        using var client = CreateClient(option);
        var url = BuildUrl(option);
        url = BuildGetUrl(url, option.Parameters);
        _logger?.LogInformation("start http get request:{url}", url);
        return await client.GetStreamAsync(url);
    }

    public async Task<HttpResult<T>> PostAsync<T>(JsonOption option, CancellationToken? cancellationToken = null)
    {
        using var client = CreateClient(option);
        var content = new StringContent(option.Parameters ?? "{}");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var url = BuildUrl(option);
        _logger?.LogInformation("start http post json request:{url}", url);
        return await Retry<T>(option, () => client.PostAsync(url, content, cancellationToken ?? new CancellationToken()));
    }

    public async Task<HttpResult<T>> PostFormAsync<T>(FormOption option, CancellationToken? cancellationToken = null)
    {
        using var client = CreateClient(option);
        HttpContent content;
        if (option.IsUrlEncoded)
        {
            content = new FormUrlEncodedContent(option.Parameters ?? new Dictionary<string, string>());
        }
        else
        {
            var formDataContent = new MultipartFormDataContent();
            if (option.Parameters.NotNull())
            {
                foreach (var parameter in option.Parameters!)
                {
                    formDataContent.Add(new StringContent(parameter.Value), parameter.Key);
                }
            }
            if (option.Files.NotEmpty())
            {
                foreach (var file in option.Files!)
                {
                    if (file.FileStream is not null) formDataContent.Add(new StreamContent(file.FileStream), file.ParameterName, file.FileName);
                    else if (file.FileData is not null) formDataContent.Add(new ByteArrayContent(file.FileData), file.ParameterName, file.FileName);
                }
            }
            content = formDataContent;
        }
        var url = BuildUrl(option);
        _logger?.LogInformation("start http post form request:{url}", url);
        return await Retry<T>(option, () => client.PostAsync(url, content, cancellationToken ?? new CancellationToken()));
    }

    public async Task<HttpResult<T>> PutAsync<T>(JsonOption option, CancellationToken? cancellationToken = null)
    {
        using var client = CreateClient(option);
        var content = new StringContent(option.Parameters ?? "{}");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var url = BuildUrl(option);
        _logger?.LogInformation("start http put request:{url}", url);
        return await Retry<T>(option, () => client.PutAsync(url, content, cancellationToken ?? new CancellationToken()));
    }

    public async Task<HttpResult<T>> DeleteAsync<T>(ParameterOption option, CancellationToken? cancellationToken = null)
    {
        using var client = CreateClient(option);
        var url = BuildUrl(option);
        url = BuildGetUrl(url, option.Parameters);
        _logger?.LogInformation("start http delete request:{url}", url);
        return await Retry<T>(option, () => client.DeleteAsync(url, cancellationToken ?? new CancellationToken()));
    }

    private HttpClient CreateClient(HttpOption option)
    {
        var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
        if (option.Cookies!.NotEmpty())
        {
            handler.CookieContainer = new CookieContainer();
            foreach (var cookie in option.Cookies!)
            {
                handler.CookieContainer.Add(cookie);
            }
        }
        var progressHanlder = new ProgressMessageHandler(handler);
        if (option.OnSendProgress.NotNull())
        {
            var speed = new TransferFileStatisticsModel(DateTime.Now);
            int lastProgress = 0;
            progressHanlder.HttpSendProgress += (_, e) =>
            {
                var progress = new HttpProgress(e.TotalBytes ?? 0, e.BytesTransferred, speed);
                if (progress.Progress > lastProgress)
                {
                    lastProgress = progress.Progress;
                    option.OnSendProgress!.Invoke(progress);
                }
            };
        }
        if (option.OnReceiveProgress.NotNull())
        {
            var speed = new TransferFileStatisticsModel(DateTime.Now);
            int lastProgress = 0;
            progressHanlder.HttpReceiveProgress += (_, e) =>
            {
                var progress = new HttpProgress(e.TotalBytes ?? 0, e.BytesTransferred, speed);
                if (progress.Progress > lastProgress)
                {
                    lastProgress = progress.Progress;
                    option.OnReceiveProgress!.Invoke(progress);
                }
            };
        }

        var client = new HttpClient(progressHanlder);
        if (option.Headers!.NotEmpty())
        {
            foreach (var header in option.Headers!)
            {
                if (!string.IsNullOrWhiteSpace(header.Value)) client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
        client.Timeout = option.TimeOut ?? _globalOptions?.TimeOut ?? TimeSpan.FromDays(1);
        return client;
    }

    private string BuildUrl(HttpOption option)
    {
        if (option.Url.IsNull()) return string.Empty;
        if (Uri.IsWellFormedUriString(option.Url, UriKind.Absolute)) return option.Url!;
        if (_globalOptions.NotNull() && _globalOptions!.BaseUrl.NotNull()) return _globalOptions.BaseUrl.CombinePath(option.Url);
        return option.Url!;
    }

    private static string BuildGetUrl(string url, Dictionary<string, string>? parameters)
    {
        if (parameters!.IsEmpty()) return url;
        return $"{url}?{(string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}")))}";
    }

    private async Task<HttpResult<T>> Retry<T>(HttpOption option, Func<Task<HttpResponseMessage>> action)
    {
        var retryCount = option.RetryCount ?? _globalOptions?.RetryCount ?? 0;
        var index = 0;
        var startTime = DateTime.Now;
        HttpResponseMessage response = default!;
        string? exceptionMessage = null;
        do
        {
            try
            {
                response = await action.Invoke();
                if (!response.IsSuccessStatusCode)
                {
                    index++;
                    continue;
                }
                return await BuildResult<T>(option, response, index, DateTime.Now - startTime, null);
            }
            catch (Exception ex)
            {
                index++;
                exceptionMessage = ex.Message;
            }
        } while (index < retryCount);
        return await BuildResult<T>(option, response, index, DateTime.Now - startTime, exceptionMessage);
    }

    private static async Task<HttpResult<T>> BuildResult<T>(HttpOption option, HttpResponseMessage response, int retryCount, TimeSpan timeConsuming, string? exceptionMessage)
    {
        if (option.Url.IsNull()) throw new ArgumentNullException(nameof(option.Url));
        var host = new Uri(option.Url).Host;
        if (response.IsNull()) return new HttpResult<T>(false, HttpStatusCode.ServiceUnavailable, exceptionMessage ?? "no response", default!, new Dictionary<string, string>(), null!, retryCount, timeConsuming);
        var cookies = (response.Headers.Contains("Set-Cookie") ? response.Headers.GetValues("Set-Cookie")?.SelectMany(x =>
        {
            if (CookieHeaderValue.TryParse(x, out var y))
            {
                return y.Cookies.Select(z => new Cookie(z.Name, z.Value, y.Path, host)).ToList();
            }
            return new List<Cookie>();
        }).Where(x => x.NotNull()).ToList() : null) ?? new List<Cookie>();

        var headers = new Dictionary<string, string>();
        foreach (var header in response.Headers)
        {
            headers.Add(header.Key, string.Join(";", header.Value));
        }

        var content = response.Content;
        if (content is null) return new HttpResult<T>(response.IsSuccessStatusCode, response.StatusCode, "empty response", default!, headers, cookies, retryCount, timeConsuming);
        var message = string.Empty;
        var stringContent = string.Empty;
        var data = default(T);
        if (!response.IsSuccessStatusCode)
        {
            var responseText = await content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<Result>(responseText);
                message = result?.Description ?? responseText;
                stringContent = responseText;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"response '{responseText}' is not json,can not convert,exception:{ex.Message}");
                message = responseText;
            }
        }
        else
        {
            var type = typeof(T);
            if (type.IsValueType)
            {
                var str = await content.ReadAsStringAsync();
                if (type.IsEnum) data = Enum.TryParse(type, str ?? string.Empty, out var x) ? (T)x! : throw new InvalidCastException($"unable to cast value '{str}' to enum type '{type.FullName}'");
                else data = (T)Convert.ChangeType(str, type);
            }
            else if (type == typeof(string)) data = (T)Convert.ChangeType(await content.ReadAsStringAsync(), type);
            else if (type == typeof(byte[]))
            {
                data = (T)Convert.ChangeType(await content.ReadAsByteArrayAsync(), type);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
            }
        }
        if (string.IsNullOrWhiteSpace(message)) message = response.StatusCode.ToString();
        return new HttpResult<T>(response.IsSuccessStatusCode, response.StatusCode, message, data!, headers, cookies, retryCount, timeConsuming) { StringContent = stringContent };
    }
}
