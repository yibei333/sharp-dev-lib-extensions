using DocumentFormat.OpenXml.Spreadsheet;

namespace SharpDevLib.Extensions.Excel.Styles;

/// <summary>
/// cell style
/// </summary>
public abstract class CellStyle
{
    /// <summary>
    /// indicate font is bold
    /// </summary>
    public bool? Bold { get; protected set; }
    /// <summary>
    /// font size
    /// </summary>
    public uint? FontSize { get; protected set; }
    /// <summary>
    /// font color(hexstring)
    /// </summary>
    public string? Color { get; protected set; }
    /// <summary>
    /// background color(hexstring)
    /// </summary>
    public string? BackgroundColor { get; protected set; }
    /// <summary>
    /// horizontal alignment
    /// </summary>
    public HorizontalAlignmentValues? HorizontalAlignment { get; protected set; }
    /// <summary>
    /// vertical alignment
    /// </summary>
    public VerticalAlignmentValues? VerticalAlignment { get; protected set; }
    /// <summary>
    /// wrap text
    /// </summary>
    public bool? WrapText { get; protected set; }
    /// <summary>
    /// border style
    /// </summary>
    public BorderStyleValues? BorderStyle { get; protected set; }
    /// <summary>
    /// border color(hexstring)
    /// </summary>
    public string? BorderColor { get; protected set; }
    /// <summary>
    /// border left style
    /// </summary>
    public BorderStyleValues? BorderLeftStyle { get; protected set; }
    /// <summary>
    /// border left color(hexstring)
    /// </summary>
    public string? BorderLeftColor { get; protected set; }
    /// <summary>
    /// border top style
    /// </summary>
    public BorderStyleValues? BorderTopStyle { get; protected set; }
    /// <summary>
    /// border top color(hexstring)
    /// </summary>
    public string? BorderTopColor { get; protected set; }
    /// <summary>
    /// border right style
    /// </summary>
    public BorderStyleValues? BorderRightStyle { get; protected set; }
    /// <summary>
    /// border right color(hexstring)
    /// </summary>
    public string? BorderRightColor { get; protected set; }
    /// <summary>
    /// border bottom style
    /// </summary>
    public BorderStyleValues? BorderBottomStyle { get; protected set; }
    /// <summary>
    /// border bottom color(hexstring)
    /// </summary>
    public string? BorderBottomColor { get; protected set; }
    /// <summary>
    /// italic font
    /// </summary>
    public bool? Italic { get; protected set; }
    /// <summary>
    /// total custom style,other property will be ignored
    /// </summary>
    public CellFormat? CellFormat { get; protected set; }
}