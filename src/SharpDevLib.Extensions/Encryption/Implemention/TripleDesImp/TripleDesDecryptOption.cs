namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// 3des decrypt option
/// </summary>
public class TripleDesDecryptOption : TripleDesEncryptOption
{
    /// <summary>
    /// instantient 3des decryption option
    /// </summary>
    /// <param name="key">decryption key</param>
    /// <param name="iv">encryption iv,ecb model not required,legnth should be BlockSize/8</param>
    public TripleDesDecryptOption(string key, byte[] iv) : base(key, iv)
    {
    }
}
