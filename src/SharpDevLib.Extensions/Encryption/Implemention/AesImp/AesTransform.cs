using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Encryption.Implemention.AesImp;

internal class AesTransform : IEncryptionTransform<AesEncryptOption, AesDecryptOption>
{
    public byte[] Decrypt(string data, AesDecryptOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        return Decrypt(Convert.FromBase64String(data), option);
    }

    private static byte[] Decrypt(byte[] data, AesDecryptOption option)
    {
        if (data.IsEmpty()) throw new Exception($"no data to decrypt");

        using var algorithm = Aes.Create();
        algorithm.KeySize = option.KeySize;
        algorithm.BlockSize = option.BlockSize;
        algorithm.Padding = option.Padding;
        algorithm.Mode = option.CipherMode;
        algorithm.Key = option.KeyBytes;
        algorithm.IV = option.IV;

        using var transform = algorithm.CreateDecryptor();
        using var memoryStream = new MemoryStream();
        using var stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write, true);
        stream.Write(data);
        stream.FlushFinalBlock();
        return memoryStream.ToArray();
    }

    public void DecryptFile(string filename, string decryptedFileName, AesDecryptOption option)
    {
        using var streamDecrypted = new FileStream(decryptedFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        DecryptFile(filename, streamDecrypted, option);
        streamDecrypted.Close();
    }

    public void DecryptFile(string filename, Stream decryptedStream, AesDecryptOption option)
    {
        using var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        using var algorithm = Aes.Create();
        algorithm.KeySize = option.KeySize;
        algorithm.BlockSize = option.BlockSize;
        algorithm.Padding = option.Padding;
        algorithm.Mode = option.CipherMode;
        algorithm.Key = option.KeyBytes;
        algorithm.IV = option.IV;

        using var transform = algorithm.CreateDecryptor();
        using var cryptoStream = new CryptoStream(decryptedStream, transform, CryptoStreamMode.Write, true);
        var buffer = new byte[4096];
        var length = -1;
        while ((length = stream.Read(buffer)) > 0)
        {
            cryptoStream.Write(buffer, 0, length);
        }
        cryptoStream.FlushFinalBlock();
        decryptedStream.Flush();
        decryptedStream.Seek(0, SeekOrigin.Begin);
    }

    public byte[] Encrypt(string data, AesEncryptOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        return Encrypt(Encoding.UTF8.GetBytes(data), option);
    }

    private static byte[] Encrypt(byte[] data, AesEncryptOption option)
    {
        if (data.IsEmpty()) throw new Exception($"no data to encrypt");

        using var algorithm = Aes.Create();
        algorithm.KeySize = option.KeySize;
        algorithm.BlockSize = option.BlockSize;
        algorithm.Padding = option.Padding;
        algorithm.Mode = option.CipherMode;
        algorithm.Key = option.KeyBytes;
        algorithm.IV = option.IV;

        using var transform = algorithm.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write, true);
        stream.Write(data);
        stream.FlushFinalBlock();
        return memoryStream.ToArray();
    }

    public void EncryptFile(string filename, string encryptedFileName, AesEncryptOption option)
    {
        using var streamEncrypted = new FileStream(encryptedFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        EncryptFile(filename, streamEncrypted, option);
        streamEncrypted.Close();
    }

    public void EncryptFile(string filename, Stream encryptedStream, AesEncryptOption option)
    {
        using var stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        using var algorithm = Aes.Create();
        algorithm.KeySize = option.KeySize;
        algorithm.BlockSize = option.BlockSize;
        algorithm.Padding = option.Padding;
        algorithm.Mode = option.CipherMode;
        algorithm.Key = option.KeyBytes;
        algorithm.IV = option.IV;

        using var transform = algorithm.CreateEncryptor();
        using var cryptoStream = new CryptoStream(encryptedStream, transform, CryptoStreamMode.Write, true);
        var buffer = new byte[4096];
        var length = -1;
        while ((length = stream.Read(buffer)) > 0)
        {
            cryptoStream.Write(buffer, 0, length);
        }
        cryptoStream.FlushFinalBlock();
        encryptedStream.Flush();
        encryptedStream.Seek(0, SeekOrigin.Begin);
    }

    public byte[] GenerateIV(byte[]? init = null)
    {
        if (init.NotEmpty()) return AesEncryptOption.PaddingKey(init!, 128 / 8);

        using var algorithm = Aes.Create();
        algorithm.GenerateIV();
        return algorithm.IV;
    }
}
