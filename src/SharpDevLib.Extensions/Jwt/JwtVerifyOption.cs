namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// json web token verify option
/// </summary>
public class JwtVerifyOption
{
    /// <summary>
    /// instantient json web token verify option
    /// </summary>
    /// <param name="token">json web token</param>
    /// <param name="key">if algorithm is RS256,key is rsa pkcs8 format public key;if algorithm is HS256,key is hmac secret</param>
    public JwtVerifyOption(string token, string key)
    {
        Token = token;
        Key = key;
    }

    /// <summary>
    /// json web token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// if algorithm is RS256,key is rsa pkcs8 format public key;if algorithm is HS256,key is hmac secret
    /// </summary>
    public string Key { get; set; }
}
