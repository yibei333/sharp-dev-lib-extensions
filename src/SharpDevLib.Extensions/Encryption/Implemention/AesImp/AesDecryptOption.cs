namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// aes decrypt option
/// </summary>
public class AesDecryptOption : AesEncryptOption
{
    /// <summary>
    /// instantient aes decryption option
    /// </summary>
    /// <param name="key">decryption key</param>
    /// <param name="iv">encryption iv,ecb model not required,legnth should be BlockSize/8</param>
    public AesDecryptOption(string key, byte[] iv) : base(key, iv)
    {
    }
}
