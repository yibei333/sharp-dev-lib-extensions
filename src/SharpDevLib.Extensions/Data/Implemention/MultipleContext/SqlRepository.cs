using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;

namespace SharpDevLib.Extensions.Data.Implemention;

internal class SqlRepository<TDbContext> : ISqlRepository<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly DbConnection _connection;

    public SqlRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
        _connection = _dbContext.Database.GetDbConnection();
        if (_connection.State != ConnectionState.Open) _connection.Open();
    }

    private IDbCommand GetCommand(string sql, params DbParameter[] parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        return command;
    }

    public void ExecuteNoneQuery(string sql, params DbParameter[] parameters)
    {
        GetCommand(sql, parameters).ExecuteNonQuery();
    }

    public IEnumerable<T> Query<T>(string sql, params DbParameter[] parameters) where T : class
    {
        var type = typeof(T);
        type.EnsureTypeContainsPublicParamterLessConstructor();
        using var reader = GetCommand(sql, parameters).ExecuteReader();
        while (reader.Read())
        {
            var instance = Activator.CreateInstance<T>() ?? throw new Exception($"unable to create instance of type '{type.FullName}'");
            var properties = type.GetProperties();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldValue = reader[i];
                if (fieldValue == null || fieldValue == DBNull.Value) continue;
                var fieldName = reader.GetName(i);
                var property = properties.FirstOrDefault(x => x.Name == fieldName);
                if (property.IsNull() || !property.PropertyType.IsPublic || property.GetCustomAttributes(typeof(NotMappedAttribute), true).Any() || !property.CanWrite) continue;
                property.SetValue(instance, fieldValue);
            }
            yield return instance;
        }
    }

    public DataSet QueryDataSet(string sql, params DbParameter[] parameters)
    {
        var factory = DbProviderFactories.GetFactory(_connection) ?? throw new Exception($"unable to get db provider factory");
        using var adapter = factory.CreateDataAdapter() ?? throw new Exception($"unable to get data adapter");
        using var command = factory.CreateCommand() ?? throw new Exception($"unable to get db command");
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        adapter.SelectCommand = command;
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet;
    }

    public T ExecuteScalar<T>(string sql, params DbParameter[] parameters)
    {
        var data = GetCommand(sql, parameters).ExecuteScalar();
        try
        {
            return (T)Convert.ChangeType(data, typeof(T))!;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"cast data[{data?.GetType()?.FullName}] to type '{typeof(T).FullName}' failed", ex);
        }
    }
}