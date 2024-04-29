namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// json web token verify result
/// </summary>
public class JwtVerifyResult
{
    internal JwtVerifyResult(bool isVerified)
    {
        IsVerified = isVerified;
    }

    internal JwtVerifyResult(bool isVerified, JwtAlgorithm algorithm, string? header, string? payload, string? signature)
    {
        IsVerified = isVerified;
        Header = header;
        Algorithm = algorithm;
        Payload = payload;
        Signature = signature;
    }

    /// <summary>
    /// is veriyied
    /// </summary>
    public bool IsVerified { get; }
    /// <summary>
    /// header
    /// </summary>
    public string? Header { get; }
    /// <summary>
    /// token algorithm
    /// </summary>
    public JwtAlgorithm Algorithm { get; }
    /// <summary>
    /// payload
    /// </summary>
    public string? Payload { get; }
    /// <summary>
    /// signature
    /// </summary>
    public string? Signature { get; }
}
