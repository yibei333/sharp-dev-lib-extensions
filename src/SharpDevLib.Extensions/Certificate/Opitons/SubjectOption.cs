namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// subject option
/// </summary>
public class SubjectOption
{
    /// <summary>
    /// instantient subject option
    /// </summary>
    /// <param name="commonName">Common Name(eg, your name or your server's hostname)</param>
    public SubjectOption(string commonName)
    {
        CommonName = commonName;
    }

    /// <summary>
    /// Country Name (2 letter code)
    /// </summary>
    public string? Country { get; set; }
    /// <summary>
    /// State or Province Name(full name)
    /// </summary>
    public string? Province { get; set; }
    /// <summary>
    /// Locality Name(eg, city)
    /// </summary>
    public string? City { get; set; }
    /// <summary>
    /// Organization Name(eg, company)
    /// </summary>
    public string? Organization { get; set; }
    /// <summary>
    /// Organizational Unit Name(eg, section)
    /// </summary>
    public string? OrganizationalUnit { get; set; }
    /// <summary>
    /// Common Name(eg, your name or your server's hostname)
    /// </summary>
    public string CommonName { get; set; }
}
