using System.Data;
using System.Data.Common;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// sql helper abstraction
/// </summary>
public interface ISqlHelper
{
    /// <summary>
    /// begin transaction
    /// </summary>
    /// <returns>DbTransaction</returns>
    DbTransaction BeginTransaction();

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>first value of result</returns>
    T? ExecuteScalar<T>(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>first value of result</returns>
    Task<T?> ExecuteScalarAsync<T>(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute scalar
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>first value of result</returns>
    Task<T?> ExecuteScalarAsync<T>(string sql,CancellationToken token, params DbParameter[] parameters);

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    DataSet ExecuteDataSet(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    Task<DataSet> ExecuteDataSetAsync(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute dataset
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>dataset</returns>
    Task<DataSet> ExecuteDataSetAsync(string sql, CancellationToken token, params DbParameter[] parameters);

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>rows count effected</returns>
    int ExecuteNonQuery(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>rows count effected</returns>
    Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] parameters);

    /// <summary>
    /// execute non query
    /// </summary>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>rows count effected</returns>
    Task<int> ExecuteNonQueryAsync(string sql, CancellationToken token, params DbParameter[] parameters);

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    List<T> QueryList<T>(string sql, params DbParameter[] parameters) where T : class;

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    Task<List<T>> QueryListAsync<T>(string sql, params DbParameter[] parameters) where T : class;

    /// <summary>
    /// query list
    /// </summary>
    /// <typeparam name="T">result type</typeparam>
    /// <param name="sql">command text</param>
    /// <param name="token">cancellation token</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>list of type T</returns>
    Task<List<T>> QueryListAsync<T>(string sql, CancellationToken token, params DbParameter[] parameters) where T : class;
}
