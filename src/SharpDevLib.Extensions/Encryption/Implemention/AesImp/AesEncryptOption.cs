using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// aes encrypt option
/// </summary>
public class AesEncryptOption
{
    /// <summary>
    /// instantient aes encryption option
    /// </summary>
    /// <param name="key">encryption key</param>
    /// <param name="iv">encryption iv,ecb model not required,legnth should be BlockSize/8</param>
    public AesEncryptOption(string key, byte[] iv)
    {
        Key = key?.Trim() ?? string.Empty;
        KeyBytes = GetKeyBytes(key!);
        KeySize = KeyBytes.Length * 8;
        BlockSize = 128;
        IV = iv;
        if (iv.Length != 16) throw new Exception("iv length should be 16");
        CipherMode = CipherMode.CBC;
        Padding = PaddingMode.PKCS7;
    }

    private static byte[] GetKeyBytes(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        //128,192,256=>16,24,32
        var bytes = Encoding.UTF8.GetBytes(key);
        var keySize = 16;
        if (bytes.Length > 16 && bytes.Length < 24) keySize = 24;
        else if (bytes.Length > 24) keySize = 32;
        return PaddingKey(bytes, keySize);
    }

    internal static byte[] PaddingKey(byte[] bytes, int keySize)
    {
        if (bytes.Length > keySize) return bytes.Take(keySize).ToArray();
        if (bytes.Length == keySize) return bytes;

        //var zeroByte = Encoding.ASCII.GetBytes("0").First();
        byte zeroByte = 0;
        var result = new byte[keySize - bytes.Length];
        for (var i = 0; i < keySize - bytes.Length; i++) result[i] = zeroByte;
        return bytes.Concat(result).ToArray();
    }

    /// <summary>
    /// encryption key,if length not enough,padding with 0x00
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// key bytes,length is 128/192/256 bit
    /// </summary>
    public byte[] KeyBytes { get; }
    /// <summary>
    /// encryption iv,ecb model not required,legnth should be BlockSize/8
    /// </summary>
    public byte[] IV { get; }
    /// <summary>
    /// key size
    /// </summary>
    public int KeySize { get; }
    /// <summary>
    /// block size,128bit length
    /// </summary>
    public int BlockSize { get; }
    /// <summary>
    /// padding model
    /// </summary>
    public PaddingMode Padding { get; set; }
    /// <summary>
    /// cipher model
    /// </summary>
    public CipherMode CipherMode { get; set; }
}
