namespace SharpDevLib.Extensions.Model;

/// <summary>
/// base request
/// </summary>
public class BaseRequest
{
}

/// <summary>
/// id request
/// </summary>
public class IdRequest : BaseRequest
{
    /// <summary>
    /// id
    /// </summary>
    public Guid Id { get; set; }
}

/// <summary>
/// name request
/// </summary>
public class NameRequest : BaseRequest
{
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// id name request
/// </summary>
public class IdNameRequest : IdRequest
{
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// data requesst
/// </summary>
/// <typeparam name="TData"></typeparam>
public class DataRequest<TData> : BaseRequest
{
    /// <summary>
    /// data
    /// </summary>
    public TData? Data { get; set; }
}

/// <summary>
/// id data requesst
/// </summary>
/// <typeparam name="TData"></typeparam>
public class IdDataRequest<TData> : IdRequest
{
    /// <summary>
    /// data
    /// </summary>
    public TData? Data { get; set; }
}

/// <summary>
/// id name data request
/// </summary>
/// <typeparam name="TData"></typeparam>
public class IdNameDataRequest<TData> : IdDataRequest<TData>
{
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// page request
/// </summary>
public class PageRequest : BaseRequest
{
    /// <summary>
    /// page index ,start with  1
    /// </summary>
    public int PageIndex { get; set; } = 1;
    /// <summary>
    /// page size
    /// </summary>
    public int PageSize { get; set; } = 20;
}