using SharpDevLib.Extensions.Excel;
using System.Data;
using System.Data.Common;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// sql helper abstraction
/// </summary>
public sealed class SqlHelper : ISqlHelper, IDisposable
{
    private readonly SqlHelperOption _option;
    private DbConnection _connection;
    private DbTransaction? _transaction;

    /// <summary>
    /// create instance of sqlhelper
    /// </summary>
    /// <param name="option">connection option</param>
    public SqlHelper(SqlHelperOption option)
    {
        _option = option ?? throw new ArgumentNullException(nameof(option));
        _connection = CreateConnection();
    }

    private DbConnection CreateConnection()
    {
        try
        {
            if (_connection is not null) return _connection;
            _connection = _option.DbProviderFactory.CreateConnection() ?? throw new Exception("create connection failed");
            _connection.ConnectionString = _option.ConnectionString.NotEmpty() ? _option.ConnectionString : _option.ConnectionStringResolver?.Resolve();
            if (_connection.ConnectionString.IsEmpty()) throw new Exception($"database connection string required");
            _connection.Open();
            return _connection;
        }
        catch (Exception ex)
        {
            throw new Exception($"create database connection failed:{ex.Message}", ex);
        }
    }

    private DbCommand CreateCommand(string sql, params DbParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        command.Transaction = _transaction;
        return command;
    }

    /// <summary>
    /// dispose the connection
    /// </summary>
    public void Dispose()
    {
        _connection?.Dispose();
    }

    /// <summary>
    /// begin transaction
    /// </summary>
    /// <returns>DbTransaction</returns>
    public DbTransaction BeginTransaction()
    {
        if (_transaction != null) return _transaction;
        _transaction = _connection.BeginTransaction();
        return _transaction;
    }

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>first value of result</returns>
    public T? ExecuteScalar<T>(string sql, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        var result = command.ExecuteScalar();
        return (T?)Convert.ChangeType(result, typeof(T?));
    }

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>first value of result</returns>
    public async Task<T?> ExecuteScalarAsync<T>(string sql, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        var result = await command.ExecuteScalarAsync();
        return (T?)Convert.ChangeType(result, typeof(T?));
    }

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>    
    /// <returns>first value of result</returns>
    public async Task<T?> ExecuteScalarAsync<T>(string sql, CancellationToken token, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        var result = await command.ExecuteScalarAsync(token);
        return (T?)Convert.ChangeType(result, typeof(T?));
    }

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    public DataSet ExecuteDataSet(string sql, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        var dataset = new DataSet();
        using var adapter = _option.DbProviderFactory.CreateDataAdapter() ?? throw new Exception("create adapter failed");
        adapter.SelectCommand = command;
        adapter.Fill(dataset);
        return dataset;
    }

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    public async Task<DataSet> ExecuteDataSetAsync(string sql, params DbParameter[] parameters)
    {
        return await Task.Run(() =>
        {
            using var command = CreateCommand(sql, parameters);
            var dataset = new DataSet();
            using var adapter = _option.DbProviderFactory.CreateDataAdapter() ?? throw new Exception("create adapter failed");
            adapter.SelectCommand = command;
            adapter.Fill(dataset);
            return dataset;
        });
    }

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    public async Task<DataSet> ExecuteDataSetAsync(string sql, CancellationToken token, params DbParameter[] parameters)
    {
        return await Task.Run(() =>
        {
            using var command = CreateCommand(sql, parameters);
            var dataset = new DataSet();
            using var adapter = _option.DbProviderFactory.CreateDataAdapter() ?? throw new Exception("create adapter failed");
            adapter.SelectCommand = command;
            adapter.Fill(dataset);
            return dataset;
        }, token);
    }

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    public int ExecuteNonQuery(string sql, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    public async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    public async Task<int> ExecuteNonQueryAsync(string sql, CancellationToken token, params DbParameter[] parameters)
    {
        using var command = CreateCommand(sql, parameters);
        return await command.ExecuteNonQueryAsync(token);
    }

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    public List<T> QueryList<T>(string sql, params DbParameter[] parameters) where T : class
    {
        var set = ExecuteDataSet(sql, parameters);
        if (set is null || set.Tables is null || set.Tables.Count <= 0) return new List<T>();
        return set.Tables[0].ToList<T>();
    }

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    public async Task<List<T>> QueryListAsync<T>(string sql, params DbParameter[] parameters) where T : class
    {
        var set = await ExecuteDataSetAsync(sql, parameters);
        if (set is null || set.Tables is null || set.Tables.Count <= 0) return new List<T>();
        return set.Tables[0].ToList<T>();
    }

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    public async Task<List<T>> QueryListAsync<T>(string sql, CancellationToken token, params DbParameter[] parameters) where T : class
    {
        var set = await ExecuteDataSetAsync(sql, token, parameters);
        if (set is null || set.Tables is null || set.Tables.Count <= 0) return new List<T>();
        return set.Tables[0].ToList<T>();
    }
}
