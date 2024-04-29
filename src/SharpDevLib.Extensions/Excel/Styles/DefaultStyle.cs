using DocumentFormat.OpenXml.Spreadsheet;

namespace SharpDevLib.Extensions.Excel.Styles;

/// <summary>
/// default style
/// </summary>
public class DefaultStyle : CellStyle
{
    /// <summary>
    /// instantient default style
    /// </summary>
    public DefaultStyle()
    {
        BackgroundColor = "FFFFFF";
        Color = "000000";
        Bold = false;
        BorderStyle = BorderStyleValues.Thin;
        BorderColor = "000000";
        FontSize = 10;
        HorizontalAlignment = HorizontalAlignmentValues.Left;
        VerticalAlignment = VerticalAlignmentValues.Top;
        Italic = false;
    }
}