namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// des decrypt option
/// </summary>
public class DesDecryptOption : DesEncryptOption
{
    /// <summary>
    /// instantient des decryption option
    /// </summary>
    /// <param name="key">decryption key</param>
    /// <param name="iv">encryption iv,ecb model not required,legnth should be BlockSize/8</param>
    public DesDecryptOption(string key, byte[] iv) : base(key, iv)
    {
    }
}
