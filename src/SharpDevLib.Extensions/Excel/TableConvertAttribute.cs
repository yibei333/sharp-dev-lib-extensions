namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// convert list to datatable header attribute
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class TableConvertAttribute : Attribute
{
    /// <summary>
    /// header key name,default is property name,if you want translate,Name will be translate key,then implement II18NService by yourself
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// is dynamic header
    /// </summary>
    public bool IsDynamic { get; set; }
    /// <summary>
    /// dynamic data index
    /// </summary>
    public uint DynamicDataIndex { get; set; }
    /// <summary>
    /// dynamic column format,default is {0}_{1}_{2},0 is daynamic data name,1 is dynamic property name,2 is container property name
    /// </summary>
    public string DynamicFormat { get; set; } = "{0}_{1}_{2}";
    /// <summary>
    /// complex column format,default is {0}_{1},0 is inner name,1 is container name
    /// </summary>
    public string ComplexFormat { get; set; } = "{0}_{1}";
    /// <summary>
    /// default value
    /// </summary>
    public object? DefaultValue { get; set; }
    /// <summary>
    /// dicimal numbers
    /// </summary>
    public uint Decimals { get; set; }
    /// <summary>
    /// datetime formart
    /// </summary>
    public string? DateFormat { get; set; }
    /// <summary>
    /// property is timestamp,convert value be datetime string
    /// </summary>
    public bool TimestampToDateTime { get; set; }
}
