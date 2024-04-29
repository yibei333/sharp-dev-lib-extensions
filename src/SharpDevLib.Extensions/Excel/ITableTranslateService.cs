namespace SharpDevLib.Extensions.Excel;

/// <summary>
/// if table convert need translate,implement this interface
/// </summary>
public interface ITableTranslateService
{
    /// <summary>
    /// get translates from keys
    /// </summary>
    /// <param name="keys">translate keys</param>
    /// <returns>translate result,if key not found,return origin key</returns>
    string[] Get(params string[] keys);
}
