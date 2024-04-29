namespace SharpDevLib.Extensions.Data;

/// <summary>
/// sql helper connection string resolver abstruction
/// </summary>
public interface ISqlHelperConnectionStringResolver
{
    /// <summary>
    /// resolve the connection string
    /// </summary>
    /// <returns>connection string</returns>
    string Resolve();
}
