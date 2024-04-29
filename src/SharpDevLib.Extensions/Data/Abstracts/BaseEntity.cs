namespace SharpDevLib.Extensions.Data;

/// <summary>
/// base entity type
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// base entity type
    /// </summary>
    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreateTime = DateTime.Now.ToUtcTimestamp();
    }

    /// <summary>
    /// primary key
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// create time
    /// </summary>
    public long CreateTime { get; set; }
}
