namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http request option with dictionary parameters
/// </summary>
public class ParameterOption : HttpOption<Dictionary<string, string>>
{
    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    public ParameterOption(string url) : base(url)
    {
    }

    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    /// <param name="parameters">parameters</param>
    public ParameterOption(string url, Dictionary<string, string> parameters) : base(url, parameters)
    {
    }
}
