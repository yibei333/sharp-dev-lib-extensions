namespace SharpDevLib.Extensions.Jwt;

/// <summary>
/// json web token service
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// create json web token
    /// </summary>
    /// <param name="option">create option</param>
    /// <returns>token</returns>
    string Create(JwtCreateOption option);

    /// <summary>
    /// verify json web token
    /// </summary>
    /// <param name="option">verify option</param>
    /// <returns>verify result</returns>
    JwtVerifyResult Verify(JwtVerifyOption option);
}