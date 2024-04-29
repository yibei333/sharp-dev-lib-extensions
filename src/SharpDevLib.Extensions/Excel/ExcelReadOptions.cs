namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// excel read options
/// </summary>
public class ExcelReadOptions
{
    /// <summary>
    /// instantient excel read options
    /// </summary>
    public ExcelReadOptions()
    {

    }

    /// <summary>
    /// instantient excel read options
    /// </summary>
    /// <param name="sheetIndex">sheetIndex,start with 0</param>
    public ExcelReadOptions(uint? sheetIndex)
    {
        SheetIndex = sheetIndex;
    }

    /// <summary>
    /// sheet index,start with 0
    /// </summary>
    public uint? SheetIndex { get; }
}
