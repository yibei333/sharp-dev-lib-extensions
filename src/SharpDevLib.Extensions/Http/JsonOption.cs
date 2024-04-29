namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http option with json parameters
/// </summary>
public class JsonOption : HttpOption<string>
{
    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    public JsonOption(string url) : base(url)
    {
    }

    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    /// <param name="parameters">parameters</param>
    public JsonOption(string url, string parameters) : base(url, parameters)
    {
    }
}