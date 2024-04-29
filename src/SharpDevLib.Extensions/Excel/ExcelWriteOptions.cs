using SharpDevLib.Extensions.Excel.Styles;
using System.Data;

namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// excel write options
/// </summary>
public class ExcelWriteOptions
{
    /// <summary>
    /// instantient excel write options
    /// </summary>
    /// <param name="table">table to write</param>
    /// <param name="sheetName">sheet name</param>
    public ExcelWriteOptions(DataTable table, string? sheetName = null)
    {
        Table = table;
        Remarks = new List<ExcelRemark>();
        SheetName = sheetName;
        ExcelStyles = new ExcelStyles();
        FileRemark = new FileRemarkOption();
    }

    /// <summary>
    /// instantient excel write options
    /// </summary>
    /// <param name="table">table to write</param>
    /// <param name="remarks">remarks</param>
    /// <param name="sheetName">sheet name</param>
    public ExcelWriteOptions(DataTable table, List<ExcelRemark> remarks, string? sheetName = null) : this(table, sheetName)
    {
        Remarks = remarks;
    }

    /// <summary>
    /// datatable
    /// </summary>
    public DataTable Table { get; }
    /// <summary>
    /// remarks
    /// </summary>
    public List<ExcelRemark> Remarks { get; }
    /// <summary>
    /// sheet name,default is 'sheet'
    /// </summary>
    public string? SheetName { get; }
    /// <summary>
    /// excel styles
    /// </summary>
    public ExcelStyles ExcelStyles { get; }
    /// <summary>
    /// file remark
    /// </summary>
    public FileRemarkOption FileRemark { get; }
}

/// <summary>
/// excel write remark
/// </summary>
public class ExcelRemark
{
    /// <summary>
    /// instantient excel write remark
    /// </summary>
    /// <param name="excelId">excel id</param>
    /// <param name="columnName">column name</param>
    /// <param name="remark">remark</param>
    /// <param name="author">commnet author</param>
    public ExcelRemark(Guid excelId, string columnName, string remark, string author = "")
    {
        ExcelId = excelId;
        ColumnName = columnName;
        Remark = remark;
        Author = author;
    }
    /// <summary>
    /// excel id
    /// </summary>
    public Guid ExcelId { get; }
    /// <summary>
    /// column name
    /// </summary>
    public string ColumnName { get; }
    /// <summary>
    /// remark
    /// </summary>
    public string Remark { get; }
    /// <summary>
    /// comment author
    /// </summary>
    public string Author { get; }
}

/// <summary>
/// excel styles
/// </summary>
public class ExcelStyles
{
    /// <summary>
    /// if true,sytles will not effected
    /// </summary>
    public bool ClearStyle { get; set; } = false;
    /// <summary>
    /// indicate column width is auto responsive
    /// </summary>
    public bool AutoCellWidth { get; set; } = true;
    /// <summary>
    /// indicate is use default head style
    /// </summary>
    public Type? Head { get; set; } = typeof(HeadStyle);
    /// <summary>
    /// row styles
    /// </summary>
    public List<RowStyleOption>? Rows { get; set; }
    /// <summary>
    /// col styles
    /// </summary>
    public List<ColStyleOption>? Cols { get; set; }
    /// <summary>
    /// cell styles
    /// </summary>
    public List<CellStyleOption>? Cells { get; set; }
}

/// <summary>
/// excel styles options base
/// </summary>
public abstract class ExcelStylesOption
{
    /// <summary>
    /// instantient excel styles option base
    /// </summary>
    /// <param name="styleType">style type,must implement 'SharpDevLib.Extensions.Excel.Styles.IExcelStyle'</param>
    public ExcelStylesOption(Type styleType)
    {
        StyleType = styleType;
    }
    /// <summary>
    /// style type,must implement 'SharpDevLib.Extensions.Excel.Styles.IExcelStyle'
    /// </summary>
    public Type StyleType { get; set; }
}

/// <summary>
/// row style option
/// </summary>
public class RowStyleOption : ExcelStylesOption
{
    /// <summary>
    /// instantient excel row styles option
    /// </summary>
    /// <param name="excelId">excel id</param>
    /// <param name="styleType">style type,must implement 'SharpDevLib.Extensions.Excel.Styles.IExcelStyle'</param>
    public RowStyleOption(Guid excelId, Type styleType) : base(styleType)
    {
        ExcelId = excelId;
    }

    /// <summary>
    /// excel id
    /// </summary>
    public Guid ExcelId { get; set; }
}

/// <summary>
/// col style option
/// </summary>
public class ColStyleOption : ExcelStylesOption
{
    /// <summary>
    /// instantient excel col styles option
    /// </summary>
    /// <param name="columnName">column name</param>
    /// <param name="styleType">style type,must implement 'SharpDevLib.Extensions.Excel.Styles.IExcelStyle'</param>
    public ColStyleOption(string columnName, Type styleType) : base(styleType)
    {
        ColumnName = columnName;
    }

    /// <summary>
    /// column name
    /// </summary>
    public string ColumnName { get; set; }
}

/// <summary>
/// cell style option
/// </summary>
public class CellStyleOption : ExcelStylesOption
{
    /// <summary>
    /// instantient excel cell styles option
    /// </summary>
    /// <param name="excelId">excel id</param>
    /// <param name="columnName">column name</param>
    /// <param name="styleType">style type,must implement 'SharpDevLib.Extensions.Excel.Styles.IExcelStyle'</param>
    public CellStyleOption(Guid excelId, string columnName, Type styleType) : base(styleType)
    {
        ExcelId = excelId;
        ColumnName = columnName;
    }

    /// <summary>
    /// excel id
    /// </summary>
    public Guid ExcelId { get; set; }
    /// <summary>
    /// column name
    /// </summary>
    public string ColumnName { get; set; }
}

/// <summary>
/// file remark option
/// </summary>
public class FileRemarkOption
{
    /// <summary>
    /// row span
    /// </summary>
    public uint? RowSpan { get; set; }
    /// <summary>
    /// col span
    /// </summary>
    public uint? ColSpan { get; set; }
    /// <summary>
    /// remark content
    /// </summary>
    public string? Content { get; set; }
}