namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// supported json web token algorithm
/// </summary>
public enum JwtAlgorithm
{
    /// <summary>
    /// HMAC SHA256
    /// </summary>
    HS256 = 1,
    /// <summary>
    /// RSA SHA256
    /// </summary>
    RS256,
}
