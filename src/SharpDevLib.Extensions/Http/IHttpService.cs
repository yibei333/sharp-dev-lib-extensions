namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http service abstraction
/// </summary>
public interface IHttpService
{
    /// <summary>
    /// http get
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="option">request option</param>
    /// <param name="cancellationToken">cancllation token</param>
    /// <returns>http response</returns>
    Task<HttpResult<T>> GetAsync<T>(ParameterOption option, CancellationToken? cancellationToken = null);

    /// <summary>
    /// http get stream
    /// </summary>
    /// <param name="option">request option</param>
    /// <returns>http response</returns>
    Task<Stream> GetStreamAsync(ParameterOption option);

    /// <summary>
    /// http post
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="option">request option</param>
    /// <param name="cancellationToken">cancllation token</param>
    /// <returns>http response</returns>
    Task<HttpResult<T>> PostAsync<T>(JsonOption option, CancellationToken? cancellationToken = null);

    /// <summary>
    /// http post form
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="option">request option</param>
    /// <param name="cancellationToken">cancllation token</param>
    /// <returns>http response</returns>
    Task<HttpResult<T>> PostFormAsync<T>(FormOption option, CancellationToken? cancellationToken = null);

    /// <summary>
    /// http put
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="option">request option</param>
    /// <param name="cancellationToken">cancllation token</param>
    /// <returns>http response</returns>
    Task<HttpResult<T>> PutAsync<T>(JsonOption option, CancellationToken? cancellationToken = null);

    /// <summary>
    /// http delete
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="option">request option</param>
    /// <param name="cancellationToken">cancllation token</param>
    /// <returns>http response</returns>
    Task<HttpResult<T>> DeleteAsync<T>(ParameterOption option, CancellationToken? cancellationToken = null);
}