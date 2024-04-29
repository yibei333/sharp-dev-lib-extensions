using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SharpDevLib.Extensions.Excel.Styles;

internal class CellStyleFactory
{
    private readonly Dictionary<Type, int> _styles = new();

    public int Create<TStyle>(WorkbookPart workbookPart) where TStyle : CellStyle
    {
        return Create(workbookPart, typeof(TStyle));
    }

    public int Create(WorkbookPart workbookPart, Type type)
    {
        if (_styles.ContainsKey(type)) return _styles[type];
        var style = (Activator.CreateInstance(type) as CellStyle) ?? throw new Exception($"type '{type.FullName}' must inherit '{typeof(CellStyle).FullName}'");

        CreateStyle(workbookPart, style);
        var styleId = (_styles.IsEmpty() ? 0 : _styles.Max(y => y.Value) + 1);
        _styles.Add(type, styleId);
        return styleId;
    }

    private static void CreateStyle(WorkbookPart workbookPart, CellStyle style)
    {
        var stylesPart = workbookPart.GetPartsOfType<WorkbookStylesPart>().FirstOrDefault() ?? workbookPart.AddNewPart<WorkbookStylesPart>();
        if (stylesPart.Stylesheet.IsNull())
        {
            stylesPart.Stylesheet = new Stylesheet
            {
                Fonts = new Fonts(),
                Fills = new Fills(),
                Borders = new Borders(),
                CellFormats = new CellFormats()
            };
        }

        if (style.CellFormat.NotNull())
        {
            stylesPart.Stylesheet.CellFormats!.AppendChild(style.CellFormat);
            return;
        }

        var defaultStyle = new DefaultStyle();

        // fonts
        var font = new Font() { Italic = (style.Italic ?? defaultStyle.Italic ?? false) ? new Italic() : null, Color = new Color { Rgb = WrapColor(style.Color ?? defaultStyle.Color) }, FontSize = new FontSize() { Val = style.FontSize ?? defaultStyle.FontSize }, Bold = new Bold { Val = style.Bold ?? defaultStyle.Bold } };
        stylesPart.Stylesheet.Fonts!.AppendChild(font);

        // fills
        if (style.GetType() == typeof(DefaultStyle))
        {
            stylesPart.Stylesheet.Fills!.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
        }
        var solidRed = new PatternFill() { PatternType = PatternValues.Solid };
        solidRed.ForegroundColor = new ForegroundColor { Rgb = WrapColor(style.BackgroundColor ?? defaultStyle.BackgroundColor) };
        var fill = new Fill { PatternFill = solidRed };
        stylesPart.Stylesheet.Fills!.AppendChild(fill);

        // borders
        var border = new Border
        {
            Outline = true,
            LeftBorder = new LeftBorder { Style = style.BorderLeftStyle ?? style.BorderStyle ?? defaultStyle.BorderLeftStyle ?? defaultStyle.BorderStyle, Color = new Color { Rgb = WrapColor(style.BorderLeftColor ?? style.BorderColor ?? defaultStyle.BorderLeftColor ?? defaultStyle.BorderColor) } },
            TopBorder = new TopBorder { Style = style.BorderTopStyle ?? style.BorderStyle ?? defaultStyle.BorderTopStyle ?? defaultStyle.BorderStyle, Color = new Color { Rgb = WrapColor(style.BorderTopColor ?? style.BorderColor ?? defaultStyle.BorderTopColor ?? defaultStyle.BorderColor) } },
            RightBorder = new RightBorder { Style = style.BorderRightStyle ?? style.BorderStyle ?? defaultStyle.BorderRightStyle ?? defaultStyle.BorderStyle, Color = new Color { Rgb = WrapColor(style.BorderRightColor ?? style.BorderColor ?? defaultStyle.BorderRightColor ?? defaultStyle.BorderColor) } },
            BottomBorder = new BottomBorder { Style = style.BorderBottomStyle ?? style.BorderStyle ?? defaultStyle.BorderBottomStyle ?? defaultStyle.BorderStyle, Color = new Color { Rgb = WrapColor(style.BorderBottomColor ?? style.BorderColor ?? defaultStyle.BorderBottomColor ?? defaultStyle.BorderColor) } },
        };
        stylesPart.Stylesheet.Borders!.AppendChild(border);

        //alignment
        var alignment = new Alignment { Horizontal = style.HorizontalAlignment ?? defaultStyle.HorizontalAlignment, Vertical = style.VerticalAlignment ?? defaultStyle.VerticalAlignment, WrapText = new BooleanValue(style.WrapText ?? defaultStyle.WrapText ?? false) };

        // styles(style index)
        var cellFomart = new CellFormat
        {
            Alignment = alignment,
            FontId = (uint)(stylesPart.Stylesheet.Fonts.Elements<Font>().ToList().IndexOf(font!)),
            FillId = (uint)(stylesPart.Stylesheet.Fills.Elements<Fill>().ToList().IndexOf(fill!)),
            BorderId = (uint)(stylesPart.Stylesheet.Borders.Elements<Border>().ToList().IndexOf(border!))
        };
        stylesPart.Stylesheet.CellFormats!.AppendChild(cellFomart);
    }

    private static HexBinaryValue? WrapColor(string? color)
    {
        if (color.IsNull()) return null;
        if (color!.Length == 6) return HexBinaryValue.FromString($"FF{color}");
        if (color.Length == 8)
        {
            if (color.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) return HexBinaryValue.FromString($"00{color[2..]}");
            return HexBinaryValue.FromString(color);
        }
        throw new Exception($"invalid color('{color}') length:{color.Length}");
    }
}
