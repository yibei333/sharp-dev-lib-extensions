namespace SharpDevLib.Extensions.Http;

/// <summary>
/// http request option with dictionary and file parameters
/// </summary>
public class FormOption : HttpOption<Dictionary<string, string>>
{
    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    public FormOption(string url) : base(url)
    {
    }

    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    /// <param name="parameters">parameters</param>
    /// <param name="isUrlEncoded">indicate content-type is application/x-wwww-formdata-encoded</param>
    public FormOption(string url, Dictionary<string, string> parameters, bool? isUrlEncoded = null) : base(url, parameters)
    {
        IsUrlEncoded = isUrlEncoded ?? false;
    }

    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    /// <param name="parameters">parameters</param>
    /// <param name="files">files</param>
    public FormOption(string url, Dictionary<string, string> parameters, List<FormFile> files) : this(url, parameters, false)
    {
        Files = files;
    }

    /// <summary>
    /// instantient http get option
    /// </summary>
    /// <param name="url">get request url</param>
    /// <param name="files">files</param>
    public FormOption(string url, List<FormFile> files) : this(url)
    {
        Files = files;
    }

    /// <summary>
    /// files
    /// </summary>
    public List<FormFile>? Files { get; }
    /// <summary>
    /// indicate content-type is application/x-wwww-formdata-encoded
    /// </summary>
    public bool IsUrlEncoded { get; }
}

/// <summary>
/// form file model
/// </summary>
public class FormFile
{
    /// <summary>
    /// instantient form file model
    /// </summary>
    /// <param name="parameterName">parameter name</param>
    /// <param name="fileName">file name</param>
    /// <param name="fileData">file byte array</param>
    public FormFile(string parameterName, string fileName, byte[] fileData)
    {
        ParameterName = parameterName;
        FileName = fileName;
        FileData = fileData;
    }
    /// <summary>
    /// instantient form file model
    /// </summary>
    /// <param name="parameterName">parameter name</param>
    /// <param name="fileName">file name</param>
    /// <param name="fileStream">file stream</param>
    public FormFile(string parameterName, string fileName, Stream fileStream)
    {
        ParameterName = parameterName;
        FileName = fileName;
        FileStream = fileStream;
    }
    /// <summary>
    /// parameter name
    /// </summary>
    public string ParameterName { get; }
    /// <summary>
    /// file name
    /// </summary>
    public string FileName { get; }
    /// <summary>
    /// file byte array
    /// </summary>
    public byte[]? FileData { get; }
    /// <summary>
    /// file stream
    /// </summary>
    public Stream? FileStream { get; }
}