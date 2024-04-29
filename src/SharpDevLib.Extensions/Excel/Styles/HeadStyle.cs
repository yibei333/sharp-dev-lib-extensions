using DocumentFormat.OpenXml.Spreadsheet;

namespace SharpDevLib.Extensions.Excel.Styles;

/// <summary>
/// head style
/// </summary>
public class HeadStyle : CellStyle
{
    /// <summary>
    /// instantient head style
    /// </summary>
    public HeadStyle()
    {
        HorizontalAlignment = HorizontalAlignmentValues.Center;
        BorderStyle = BorderStyleValues.Thin;
        BorderColor = "0c0d0e";
        BackgroundColor = "e5e5e5";
        Bold = true;
        FontSize = 10;
        Color = "000000";
    }
}
