using HeyRed.Mime;

namespace SharpDevLib.Extensions.Http;

/// <summary>
/// file extension
/// </summary>
public static class FileExtension
{
    private static readonly double _kbUnit = 1024;
    private static readonly double _mbUnit = 1024 * _kbUnit;
    private static readonly double _gbUnit = 1024 * _mbUnit;
    private static readonly double _tbUnit = 1024 * _gbUnit;

    /// <summary>
    /// transfer file size with unit
    /// </summary>
    /// <param name="size">file size</param>
    /// <returns>file size string</returns>
    public static string GetSize(this long size)
    {
        if (size > _tbUnit) return $"{(Math.Round(size / _tbUnit, 2))}TB";
        else if (size > _gbUnit) return $"{(Math.Round(size / _gbUnit, 2))}GB";
        else if (size > _mbUnit) return $"{(Math.Round(size / _mbUnit, 2))}MB";
        else if (size > _kbUnit) return $"{(Math.Round(size / _kbUnit, 2))}KB";
        else return $"{size}Byte";
    }

    /// <summary>
    /// get file mime type by file name
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <returns>mime type</returns>
    public static string GetMimeType(this string fileName) => MimeTypesMap.GetMimeType(fileName);

    /// <summary>
    /// format the file path
    /// </summary>
    /// <param name="path">file path</param>
    /// <returns>formated path</returns>
    public static string FormatPath(this string path) => path.Replace("\\", "/").TrimStart('/').Replace("\r", "").Replace("\n", "").Replace("\r\n", "");

    private static readonly char[] _separator = new[] { ',', ';' };

    /// <summary>
    /// split string to list by separator
    /// </summary>
    /// <param name="source">source string</param>
    /// <param name="lowercase">is result lowercase</param>
    /// <param name="separator">split separator,default is [',',';']</param>
    /// <returns>string list</returns>
    public static List<string> SplitToList(this string source, bool lowercase = true, char[]? separator = null) => source?.Split(separator?? _separator, StringSplitOptions.RemoveEmptyEntries).Select(x => lowercase ? x.ToLower() : x).ToList() ?? new List<string>();
}