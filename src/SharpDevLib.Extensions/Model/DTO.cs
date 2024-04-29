namespace SharpDevLib.Extensions.Model;

/// <summary>
/// base dto
/// </summary>
public class BaseDto
{
}

/// <summary>
/// id dto
/// </summary>
public class IdDto : BaseDto
{
    /// <summary>
    /// id
    /// </summary>
    public Guid Id { get; set; }
}

/// <summary>
/// name dto
/// </summary>
public class NameDto : BaseDto
{
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// id name dto
/// </summary>
public class IdNameDto : IdDto
{
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// data dto
/// </summary>
/// <typeparam name="TData">data type</typeparam>
public class DataDto<TData> : BaseDto
{
    /// <summary>
    /// data
    /// </summary>
    public TData? Data { get; set; }
}

/// <summary>
/// id data dto
/// </summary>
/// <typeparam name="TData">data type</typeparam>
public class IdDataDto<TData> : DataDto<TData>
{
    /// <summary>
    /// id
    /// </summary>
    public Guid Id { get; set; }
}

/// <summary>
/// id name data dto
/// </summary>
/// <typeparam name="TData">data type</typeparam>
public class IdNameDataDto<TData> : DataDto<TData>
{
    /// <summary>
    /// id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// name
    /// </summary>
    public string? Name { get; set; }
}