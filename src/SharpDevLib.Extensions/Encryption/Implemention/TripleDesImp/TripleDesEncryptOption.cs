using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Encryption;

/// <summary>
/// 3des encrypt option
/// </summary>
public class TripleDesEncryptOption
{
    /// <summary>
    /// instantient 3des encryption option
    /// </summary>
    /// <param name="key">encryption key,key length decide calculate times((16 or 24)/8)</param>
    /// <param name="iv">encryption iv,ecb model not required,legnth should be BlockSize/8</param>
    public TripleDesEncryptOption(string key, byte[] iv)
    {
        Key = key?.Trim() ?? string.Empty;
        KeyBytes = GetKeyBytes(key!);
        KeySize = KeyBytes.Length * 8;
        BlockSize = 64;
        IV = iv;
        if (iv.Length != 8) throw new Exception("iv length should be 8");
        CipherMode = CipherMode.CBC;
        Padding = PaddingMode.PKCS7;
    }

    private static byte[] GetKeyBytes(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        //128,192=>16,24
        var bytes = Encoding.UTF8.GetBytes(key);
        var keySize = 16;
        if (bytes.Length > 16) keySize = 24;
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
    /// key bytes,length is 64 bit
    /// </summary>
    public byte[] KeyBytes { get; }
    /// <summary>
    /// encryption iv,ecb model not required,legnth should be BlockSize/8
    /// </summary>
    public byte[] IV { get; }
    /// <summary>
    /// key size,128/192 bit length
    /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.tripledes.key?view=net-6.0
    /// </summary>
    public int KeySize { get; }
    /// <summary>
    /// block size,64 bit length
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
