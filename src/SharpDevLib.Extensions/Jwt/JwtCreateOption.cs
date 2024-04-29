namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// json web token create option
/// </summary>
public class JwtCreateOption
{
    /// <summary>
    /// instantient json web token creation option
    /// </summary>
    /// <param name="algorithm">jwt algorithm</param>
    /// <param name="key">if algorithm is RS256,key is rsa pkcs8 format private key;if algorithm is HS256,key is hmac secret</param>
    /// <param name="payload">payload</param>
    public JwtCreateOption(JwtAlgorithm algorithm, string key, object payload)
    {
        Algorithm = algorithm;
        Key = key;
        Payload = payload;
    }

    /// <summary>
    /// jwt algorithm
    /// </summary>
    public JwtAlgorithm Algorithm { get; }

    /// <summary>
    /// if algorithm is RS256,key is rsa pkcs8 format private key;if algorithm is HS256,key is hmac secret
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// payload
    /// </summary>
    public object Payload { get; }
}
