using System.Data.Common;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// sql helper option
/// </summary>
public class SqlHelperOption
{
    /// <summary>
    /// create instance of SqlHelperOption 
    /// </summary>
    /// <param name="dbProviderFactory">db provider factory</param>
    /// <param name="connectionString">database connection string</param>
    public SqlHelperOption(DbProviderFactory dbProviderFactory, string connectionString)
    {
        ConnectionString = connectionString;
        DbProviderFactory = dbProviderFactory;
    }

    /// <summary>
    /// create instance of SqlHelperOption 
    /// </summary>
    /// <param name="dbProviderFactory">db provider factory</param>
    /// <param name="connectionStringResolver">database connection string resolver,effect when ConnectionString is null or emtpy</param>
    public SqlHelperOption(DbProviderFactory dbProviderFactory, ISqlHelperConnectionStringResolver connectionStringResolver)
    {
        ConnectionStringResolver = connectionStringResolver;
        DbProviderFactory = dbProviderFactory;
    }

    /// <summary>
    /// database connection string
    /// </summary>
    public string? ConnectionString { get; }

    /// <summary>
    /// database connection string resolver,effect when ConnectionString is null or emtpy
    /// </summary>
    public ISqlHelperConnectionStringResolver? ConnectionStringResolver { get; }

    /// <summary>
    /// db provider factory
    /// </summary>
    public DbProviderFactory DbProviderFactory { get; }
}
