namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// read/write model or datatable must contains ExcelId field
/// </summary>
public interface IExcelId
{
    /// <summary>
    /// excel id to find row
    /// </summary>
    Guid ExcelId { get; set; }
}
