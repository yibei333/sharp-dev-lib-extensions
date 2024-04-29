using DocumentFormat.OpenXml.Spreadsheet;

namespace SharpDevLib.Extensions.Excel;

internal class CellModel
{
    public CellModel(Guid excelId, uint rowIndex, uint columnIndex, string columnName, string cellReference, Cell cell, bool isHead)
    {
        ExcelId = excelId;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        ColumnName = columnName;
        CellReference = cellReference;
        Cell = cell;
        IsHead = isHead;
    }

    public Guid ExcelId { get; }
    public uint RowIndex { get; }
    public uint ColumnIndex { get; }
    public string ColumnName { get; }
    public string CellReference { get; }
    public Cell Cell { get; }
    public bool IsHead { get; }
}
