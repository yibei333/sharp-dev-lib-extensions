using System.Data;
using System.Data.Common;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// data repository sql operation abstraction
/// </summary>
public interface ISqlRepository
{
    /// <summary>
    /// query data from sql and map to model
    /// </summary>
    /// <typeparam name="T">result model type</typeparam>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>data collection</returns>
    IEnumerable<T> Query<T>(string sql, params DbParameter[] parameters) where T : class;
    /// <summary>
    /// query datatable from sql
    /// </summary>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql parameters</param>
    /// <returns>datatable</returns>
    DataSet QueryDataSet(string sql, params DbParameter[] parameters);
    /// <summary>
    /// execute none query command
    /// </summary>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql parameters</param>
    void ExecuteNoneQuery(string sql, params DbParameter[] parameters);
    /// <summary>
    /// execute scalar
    /// </summary>
    /// <param name="sql">sql</param>
    /// <param name="parameters">sql parameters</param>
    T ExecuteScalar<T>(string sql, params DbParameter[] parameters);

    ///// <summary>
    ///// copy datatable to temporary table
    ///// </summary>
    ///// <param name="table"></param>
    ///// <param name="tableName"></param>
    //void CopyToTempTable(DataTable table,string tableName);
}