namespace SharpDevLib.Extensions.Excel.Styles;

/// <summary>
/// file remark style
/// </summary>
public class FileRemarkStyle : CellStyle
{
    /// <summary>
    /// instantient file remark style
    /// </summary>
    public FileRemarkStyle()
    {
        HorizontalAlignment = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left;
        VerticalAlignment = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Top;
        WrapText = true;
        Color = "FF0000";
        BackgroundColor = "ffe4c4";
    }
}
