using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// table and list convert extension
/// </summary>
public static class TableConvertExtension
{
    #region Common
    private static List<PropertyInfo> GetConvertProperties(this Type type) => type.GetProperties().Where(x => x.GetCustomAttribute<NotMappedAttribute>().IsNull()).ToList();

    private static bool IsDynamicColumn(this PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        return attribute.NotNull() && attribute!.IsDynamic;
    }

    private static bool IsComplexType(this PropertyInfo property)
    {
        return property.PropertyType.IsClass && property.PropertyType != typeof(string);
    }

    private static string GetColumnName(this PropertyInfo property, TableConvertAttribute? attribute, ITableTranslateService? transalte)
    {
        var key = attribute?.Name ?? property.Name;
        return transalte?.Get(key).First() ?? key;
    }

    private static Type GetDataColumnType(this PropertyInfo property, TableConvertAttribute? attribute)
    {
        return ((property.PropertyType.IsNumeric() && attribute?.Decimals > 0) || property.PropertyType.IsDateTime() || (attribute?.TimestampToDateTime ?? false)) ? typeof(string) : property.PropertyType;
    }

    private static bool IsDateTime(this Type? type)
    {
        if (type.IsNull()) return false;
        return Type.GetTypeCode(type) switch
        {
            TypeCode.DateTime => true,
            TypeCode.Object => (type!.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) && IsDateTime(Nullable.GetUnderlyingType(type)),
            _ => false
        };
    }

    /// <summary>
    /// indicate type is a numeric type
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>is a numeric type</returns>
    public static bool IsNumeric(this Type? type)
    {
        if (type.IsNull()) return false;
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte => true,
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.SByte => true,
            TypeCode.Single => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            TypeCode.Object => (type!.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) && IsNumeric(Nullable.GetUnderlyingType(type)),
            _ => false
        };
    }
    #endregion

    #region list to datatable
    /// <summary>
    /// convert collection to datatable
    /// </summary>
    /// <typeparam name="T">collection type</typeparam>
    /// <param name="collection">collection data</param>
    /// <returns>datatable</returns>
    public static DataTable ToTable<T>(this IEnumerable<T> collection) where T : class => collection.ToTable<T>(null, null);

    /// <summary>
    /// convert collection to datatable
    /// </summary>
    /// <typeparam name="T">collection type</typeparam>
    /// <param name="collection">collection data</param>
    /// <param name="translateService">cache for translate column name</param>
    /// <returns>datatable</returns>
    public static DataTable ToTable<T>(this IEnumerable<T> collection, ITableTranslateService translateService) where T : class => collection.ToTable<T>(null, translateService);

    /// <summary>
    /// convert collection to datatable
    /// </summary>
    /// <typeparam name="T">collection type</typeparam>
    /// <param name="collection">collection data</param>
    /// <param name="dynamicDatas">dynamic column data</param>
    /// <returns>datatable</returns>
    public static DataTable ToTable<T>(this IEnumerable<T> collection, IEnumerable<IDynamicKey>[] dynamicDatas) where T : class => collection.ToTable<T>(dynamicDatas, null);

    /// <summary>
    /// convert collection to datatable
    /// </summary>
    /// <typeparam name="T">collection type</typeparam>
    /// <param name="collection">collection data</param>
    /// <param name="translateService">cache for translate column name</param>
    /// <param name="dynamicDatas">dynamic column data</param>
    /// <returns>datatable</returns>
    public static DataTable ToTable<T>(this IEnumerable<T> collection, IEnumerable<IDynamicKey>[]? dynamicDatas, ITableTranslateService? translateService) where T : class
    {
        var table = new DataTable();
        table.CreateColumns(typeof(T).GetConvertProperties(), translateService, dynamicDatas);
        foreach (var item in collection) table.SetRowValues(item, translateService, dynamicDatas);
        return table;
    }

    private static void CreateColumns(this DataTable table, List<PropertyInfo> properties, ITableTranslateService? translateService, IEnumerable<IDynamicKey>[]? dynamicDatas)
    {
        properties.ForEach(property =>
        {
            if (property.IsDynamicColumn()) table.CreateDynamicColumns(property, translateService, dynamicDatas);
            else if (property.IsComplexType()) table.CreateComplexColumns(property, translateService);
            else table.CreateNormalColumns(property, translateService);
        });
    }

    private static void CreateNormalColumns(this DataTable table, PropertyInfo property, ITableTranslateService? translateService)
    {
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        table.Columns.Add(new DataColumn(property.GetColumnName(attribute, translateService), property.GetDataColumnType(attribute)));
    }

    private static void CreateDynamicColumns(this DataTable table, PropertyInfo property, ITableTranslateService? translateService, IEnumerable<IDynamicKey>[]? dynamicDatas)
    {
        if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(List<>)) throw new Exception($"dynamic property({property.Name}) type must be 'List<>'");
        var type = property.PropertyType.GetGenericArguments().First();
        var properties = type.GetConvertProperties();
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        if (dynamicDatas.IsEmpty()) throw new Exception($"could not find dynamic data for column '{attribute?.Name ?? property.Name}'");
        var dynamicDataIndex = attribute?.DynamicDataIndex ?? 0;
        var dynamicData = dynamicDatas![dynamicDataIndex];
        foreach (var data in dynamicData)
        {
            foreach (var childProperty in properties)
            {
                if (childProperty.IsDynamicColumn() || childProperty.IsComplexType()) throw new Exception($"nested dynamic or complex property({childProperty.Name}) is not supported");

                var childAttribute = childProperty.GetCustomAttribute<TableConvertAttribute>();
                table.Columns.Add(new DataColumn(string.Format(attribute?.DynamicFormat ?? "{0}_{1}_{2}", data.DynamicName, childProperty.GetColumnName(childAttribute, translateService), property.GetColumnName(attribute, translateService)), childProperty.GetDataColumnType(childAttribute)));
            }
        }
    }

    private static void CreateComplexColumns(this DataTable table, PropertyInfo property, ITableTranslateService? translateService)
    {
        if (property.PropertyType.IsGenericType || property.PropertyType.IsArray) throw new Exception($"complex property({property.Name}) not support generic type and array");
        var type = property.PropertyType;
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var properties = type.GetConvertProperties();
        foreach (var complexProperty in properties)
        {
            if (complexProperty.IsDynamicColumn() || complexProperty.IsComplexType()) throw new Exception($"nested dynamic or complex property({complexProperty.Name}) is not supported");

            var complexAttribute = complexProperty.GetCustomAttribute<TableConvertAttribute>();
            table.Columns.Add(new DataColumn(string.Format(attribute?.ComplexFormat ?? "{0}_{1}", complexProperty.GetColumnName(complexAttribute, translateService), property.GetColumnName(attribute, translateService)), complexProperty.GetDataColumnType(complexAttribute)));
        }
    }

    private static void SetRowValues<T>(this DataTable table, T instance, ITableTranslateService? translateService, IEnumerable<IDynamicKey>[]? dynamicDatas) where T : class
    {
        var row = table.NewRow();
        typeof(T).GetConvertProperties().ForEach(property =>
        {
            if (property.IsDynamicColumn()) row.SetDynamicRowValue(instance, property, translateService, dynamicDatas);
            else if (property.IsComplexType()) row.SetComplexRowValue(instance, property, translateService);
            else row.SetNormalRowValue(instance, property, translateService);
        });
        table.Rows.Add(row);
    }

    private static void SetNormalRowValue<T>(this DataRow row, T instance, PropertyInfo property, ITableTranslateService? translateService)
    {
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var name = property.GetColumnName(attribute, translateService);
        row[name] = property.GetRowValue(instance, attribute);
    }

    private static void SetDynamicRowValue<T>(this DataRow row, T instance, PropertyInfo property, ITableTranslateService? translateService, IEnumerable<IDynamicKey>[]? dynamicDatas)
    {
        var type = property.PropertyType.GetGenericArguments().First();
        if (type.GetInterface(typeof(IDynamicKey).FullName!).IsNull()) throw new Exception($"type({type.FullName}) must implement '{typeof(IDynamicKey).FullName}'");
        var properties = type.GetConvertProperties();
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var dynamicDataIndex = attribute?.DynamicDataIndex ?? 0;
        var dynamicData = dynamicDatas![dynamicDataIndex];
        var collection = property.GetValue(instance) as IEnumerable<IDynamicKey>;
        foreach (var data in dynamicData)
        {
            foreach (var childProperty in properties)
            {
                if (childProperty.IsDynamicColumn() || childProperty.IsComplexType()) throw new Exception($"nested dynamic or complex property({childProperty.Name}) is not supported");

                var childAttribute = childProperty.GetCustomAttribute<TableConvertAttribute>();
                var name = string.Format(attribute?.DynamicFormat ?? "{0}_{1}_{2}", data.DynamicName, childProperty.GetColumnName(childAttribute, translateService), property.GetColumnName(attribute, translateService));
                var dynamicInstance = collection?.FirstOrDefault(x => x.DynamicName == data.DynamicName);
                row[name] = childProperty.GetRowValue(dynamicInstance as object, childAttribute);
            }
        }
    }

    private static void SetComplexRowValue<T>(this DataRow row, T instance, PropertyInfo property, ITableTranslateService? translateService)
    {
        var type = property.PropertyType;
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var properties = type.GetConvertProperties();
        var complexInstance = property.GetValue(instance);
        foreach (var complexProperty in properties)
        {
            var complexAttribute = complexProperty.GetCustomAttribute<TableConvertAttribute>();
            var name = string.Format(attribute?.ComplexFormat ?? "{0}_{1}", complexProperty.GetColumnName(complexAttribute, translateService), property.GetColumnName(attribute, translateService));
            row[name] = complexProperty.GetRowValue(complexInstance, complexAttribute);
        }
    }

    private static object GetRowValue(this PropertyInfo property, object? instance, TableConvertAttribute? attribute)
    {
        var value = instance.IsNull() ? attribute?.DefaultValue : property.GetValue(instance) ?? attribute?.DefaultValue;
        if (value.IsNull()) return DBNull.Value;
        if (property.PropertyType.IsDateTime())
        {
            var dateValue = (DateTime)value!;
            return dateValue.ToString(attribute?.DateFormat, CultureInfo.CurrentCulture);
        }
        if (attribute?.TimestampToDateTime ?? false)
        {
            var dateValue = long.Parse(value!.ToString()!).ToUtcTime().ToLocalTime();
            return dateValue.ToString(attribute?.DateFormat, CultureInfo.CurrentCulture);
        }
        if (property.PropertyType.IsNumeric() && attribute?.Decimals > 0)
        {
            var format = $"{{0:N{attribute?.Decimals}}}";
            return string.Format(format, value);
        }
        return value!;
    }
    #endregion

    #region datatable to list
    /// <summary>
    /// convert datatable to list
    /// </summary>
    /// <typeparam name="T">list data type</typeparam>
    /// <param name="table">datatable</param>
    /// <returns>list</returns>
    public static List<T> ToList<T>(this DataTable table) where T : class => table.ToList<T>(null, null);

    /// <summary>
    /// convert datatable to list
    /// </summary>
    /// <typeparam name="T">list data type</typeparam>
    /// <param name="table">datatable</param>
    /// <param name="translateService">cache for translate column name</param>
    /// <returns>list</returns>
    public static List<T> ToList<T>(this DataTable table, ITableTranslateService translateService) where T : class => table.ToList<T>(null, translateService);

    /// <summary>
    /// convert datatable to list
    /// </summary>
    /// <typeparam name="T">list data type</typeparam>
    /// <param name="table">datatable</param>
    /// <param name="dynamicDatas">dynamic column data</param>
    /// <returns>list</returns>
    public static List<T> ToList<T>(this DataTable table, IEnumerable<IDynamicKey>[] dynamicDatas) where T : class => table.ToList<T>(dynamicDatas, null);

    /// <summary>
    /// convert datatable to list
    /// </summary>
    /// <typeparam name="T">list data type</typeparam>
    /// <param name="table">datatable</param>
    /// <param name="translateService">cache for translate column name</param>
    /// <param name="dynamicDatas">dynamic column data</param>
    /// <returns>list</returns>
    public static List<T> ToList<T>(this DataTable table, IEnumerable<IDynamicKey>[]? dynamicDatas, ITableTranslateService? translateService) where T : class
    {
        var result = new List<T>();
        foreach (DataRow row in table.Rows)
        {
            var instance = Activator.CreateInstance<T>() ?? throw new Exception($"can not create instance of type '{typeof(T).FullName}'");
            instance.MapRowValue(row, dynamicDatas, translateService);
            result.Add(instance);
        }
        return result;
    }

    private static void MapRowValue<T>(this T instance, DataRow row, IEnumerable<IDynamicKey>[]? dynamicDatas, ITableTranslateService? translateService) where T : class
    {
        var properties = instance.GetType().GetConvertProperties();
        foreach (var property in properties)
        {
            if (property.IsDynamicColumn()) instance.MapDynamicRowValue(property, row, dynamicDatas, translateService);
            else if (property.IsComplexType()) instance.MapComplexRowValue(property, row, translateService);
            else instance.MapNormalRowValue(property, row, translateService);
        }
    }

    private static void MapNormalRowValue<T>(this T instance, PropertyInfo property, DataRow row, ITableTranslateService? translateService) where T : class
    {
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var name = property.GetColumnName(attribute, translateService);
        if (!row.Table.Columns.Contains(name)) return;
        var value = row[name];
        property.SetConvertValue(instance, value);
    }

    private static void MapDynamicRowValue<T>(this T instance, PropertyInfo property, DataRow row, IEnumerable<IDynamicKey>[]? dynamicDatas, ITableTranslateService? translateService) where T : class
    {
        if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(List<>)) throw new Exception($"dynamic property({property.Name}) type must be 'List<>'");
        var type = property.PropertyType.GetGenericArguments().First();
        var properties = type.GetConvertProperties();
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        if (dynamicDatas.IsEmpty()) throw new Exception($"could not find dynamic data for column '{attribute?.Name ?? property.Name}'");
        var dynamicDataIndex = attribute?.DynamicDataIndex ?? 0;
        var dynamicData = dynamicDatas![dynamicDataIndex];

        var childCollection = Activator.CreateInstance(property.PropertyType);
        property.SetValue(instance, childCollection);
        foreach (var data in dynamicData)
        {
            var childInstance = Activator.CreateInstance(type) as IDynamicKey;
            childInstance!.DynamicName = data.DynamicName;

            foreach (var childProperty in properties)
            {
                if (childProperty.IsDynamicColumn() || childProperty.IsComplexType()) throw new Exception($"nested dynamic or complex property({childProperty.Name}) is not supported");

                var childAttribute = childProperty.GetCustomAttribute<TableConvertAttribute>();
                var name = string.Format(attribute?.DynamicFormat ?? "{0}_{1}_{2}", data.DynamicName, childProperty.GetColumnName(childAttribute, translateService), property.GetColumnName(attribute, translateService));
                var value = row[name];
                childProperty.SetConvertValue(childInstance, value);
            }
            var addMethod = property.PropertyType.GetMethod("Add");
            addMethod!.Invoke(childCollection, new[] { childInstance });
        }
    }

    private static void MapComplexRowValue<T>(this T instance, PropertyInfo property, DataRow row, ITableTranslateService? translateService) where T : class
    {
        if (property.PropertyType.IsGenericType || property.PropertyType.IsArray)
        {
            //try set by normal
            try
            {
                instance.MapNormalRowValue(property, row, translateService);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"complex property({property.Name}) not support generic type and array");
            }
        }
        var type = property.PropertyType;
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        var properties = type.GetConvertProperties();

        var complexInstance = Activator.CreateInstance(type);
        property.SetValue(instance, complexInstance);
        foreach (var complexProperty in properties)
        {
            if (complexProperty.IsDynamicColumn() || complexProperty.IsComplexType()) throw new Exception($"nested dynamic or complex property({complexProperty.Name}) is not supported");

            var complexAttribute = complexProperty.GetCustomAttribute<TableConvertAttribute>();
            var name = string.Format(attribute?.ComplexFormat ?? "{0}_{1}", complexProperty.GetColumnName(complexAttribute, translateService), property.GetColumnName(attribute, translateService));
            var value = row[name];
            complexProperty.SetConvertValue(complexInstance, value);
        }
    }

    private static void SetConvertValue(this PropertyInfo property, object? instance, object? value)
    {
        if (instance.IsNull()) return;
        var attribute = property.GetCustomAttribute<TableConvertAttribute>();
        if (attribute?.TimestampToDateTime ?? false)
        {
            var dateConvertedValue = (value.IsNullValue()) ? attribute?.DefaultValue : Convert.ToDateTime(value).ToUtcTimestamp();
            property.SetValue(instance, dateConvertedValue);
            return;
        }

        if (value.IsNullValue())
        {
            property.SetDefaultValue(attribute, instance);
            return;
        }

        if (property.PropertyType.IsGenericType)
        {
            if (property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) throw new Exception($"not supported generic type('{property.PropertyType.FullName}') convert");
            var genericType = property.PropertyType.GetGenericArguments()[0];
            property.SetValue(instance, value.ConvertedValue(genericType, attribute));
            return;
        }
        property.SetValue(instance, value.ConvertedValue(property.PropertyType, attribute));
    }

    private static bool IsNullValue(this object? value) => value.IsNull() || value.IsDbNull() || value!.ToString().IsNull();

    private static void SetDefaultValue(this PropertyInfo property, TableConvertAttribute? attribute, object? instance)
    {
        var defaultValue = attribute?.DefaultValue ?? (property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null);
        property.SetValue(instance, defaultValue);
    }

    private static object? ConvertedValue(this object? value, Type targetType, TableConvertAttribute? attribute)
    {
        if (targetType == typeof(Guid)) return Guid.TryParse(value?.ToString(), out var id) ? id : Guid.Empty;
        return Convert.ChangeType(value, targetType) ?? attribute?.DefaultValue;
    }
    #endregion
}
