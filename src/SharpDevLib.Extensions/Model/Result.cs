namespace SharpDevLib.Extensions.Model;

/// <summary>
/// result model
/// </summary>
public class Result
{
    /// <summary>
    /// is success
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// result description
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// extra data field
    /// </summary>
    public object? ExtraData { get; set; }
    /// <summary>
    /// get succeed result
    /// </summary>
    /// <param name="message">result message</param>
    /// <returns>success result</returns>
    public static Result Succeed(string? message = null) => new Result { Success = true, Description = message };
    /// <summary>
    /// get fail result
    /// </summary>
    /// <param name="message">result message</param>
    /// <returns>fail result</returns>
    public static Result Failed(string? message = null) => new Result { Success = false, Description = message };
    /// <summary>
    /// get succeed result with data
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="data">data</param>
    /// <param name="message">result message</param>
    /// <returns>success result</returns>
    public static Result<T> Succeed<T>(T data, string? message = null) => new Result<T> { Success = true, Description = message, Data = data };
    /// <summary>
    /// get fail result
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="message">result message</param>
    /// <returns>fail result</returns>
    public static Result<T> Failed<T>(string? message = null) => new Result<T> { Success = false, Description = message };
    /// <summary>
    /// get succeed result with data
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="data">data</param>
    /// <param name="total">total count</param>
    /// <param name="pageIndex">page index</param>
    /// <param name="pageSize">page size</param>
    /// <param name="message">result message</param>
    /// <returns>success result</returns>
    public static PageResult<T> SucceedPage<T>(List<T> data, long total, int pageIndex, int pageSize, string? message = null) => new PageResult<T> { Success = true, Description = message, Data = data, Total = total, PageIndex = pageIndex, PageSize = pageSize };
    /// <summary>
    /// get fail result
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <param name="message">result message</param>
    /// <returns>fail result</returns>
    public static PageResult<T> FailedPage<T>(string? message = null) => new PageResult<T> { Success = false, Description = message };
}

/// <summary>
/// result model with data
/// </summary>
/// <typeparam name="T">data type</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// data
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// page result
/// </summary>
/// <typeparam name="T">page data type</typeparam>
public class PageResult<T> : Result
{
    /// <summary>
    /// page index ,start with  1
    /// </summary>
    public int PageIndex { get; set; }
    /// <summary>
    /// page size
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// total count
    /// </summary>
    public long Total { get; set; }
    /// <summary>
    /// page count
    /// </summary>
    public long PageCount => PageSize <= 0 ? 0 : (int)Math.Ceiling(Total * 1.0d / PageSize);
    /// <summary>
    /// page data
    /// </summary>
    public List<T> Data { get; set; } = new List<T>();
}
