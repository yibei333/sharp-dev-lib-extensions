using SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;
using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Encryption.Implemention.RsaImp;

internal class RsaTransform : IRsaEncryptionTransform
{
    private readonly IRsaKey _rsaKey;

    public RsaTransform(IRsaKey rsaKey)
    {
        _rsaKey = rsaKey;
    }

    public byte[] Decrypt(string data, RsaDecryptOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        if (option.PrivateKey.IsNull()) throw new ArgumentNullException(nameof(option.PrivateKey));
        var hasPassword = _rsaKey.IsKeyHasPassword(option.PrivateKey);
        if (hasPassword && option.PrivateKeyPassword.IsNull()) throw new ArgumentNullException(nameof(option.PrivateKeyPassword));

        using var rsa = RSA.Create();
        try
        {
            var decryptedKey = option.PrivateKey;
            if (hasPassword) decryptedKey = _rsaKey.RemovePrivateKeyPassword(option.PrivateKey, option.PrivateKeyPassword!);
            var pkcs8PrivateKey = _rsaKey.ConvertType(decryptedKey, RsaKeyType.Pkcs8);
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(pkcs8PrivateKey.RemoveBeauty()), out var _);
        }
        catch (Exception ex)
        {
            throw new Exception("private key format error", ex);
        }
        return rsa.Decrypt(Convert.FromBase64String(data), option.Padding ?? RSAEncryptionPadding.Pkcs1);
    }

    public byte[] Encrypt(string data, RsaEncryptOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        if (option.PublicKey.IsNull()) throw new Exception("public key required");

        using var rsa = RSA.Create();
        try
        {
            var pkcs8PublicKey = _rsaKey.ConvertType(option.PublicKey, RsaKeyType.Pkcs8);
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pkcs8PublicKey.RemoveBeauty()), out var _);
        }
        catch (Exception ex)
        {
            throw new Exception("public key format error", ex);
        }
        var bytes = Encoding.UTF8.GetBytes(data);
        var padding = option.Padding ?? RSAEncryptionPadding.Pkcs1;
        EnsureEncryptDataLengthCorrect(rsa.KeySize, bytes, padding);
        return rsa.Encrypt(bytes, padding);
    }

    public byte[] Sign(string data, RsaSignOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        if (option.PrivateKey.IsNull()) throw new ArgumentNullException(nameof(option.PrivateKey));
        var hasPassword = _rsaKey.IsKeyHasPassword(option.PrivateKey);
        if (hasPassword && option.PrivateKeyPassword.IsNull()) throw new ArgumentNullException(nameof(option.PrivateKeyPassword));

        using var rsa = RSA.Create();
        try
        {
            var decryptedKey = option.PrivateKey;
            if (hasPassword) decryptedKey = _rsaKey.RemovePrivateKeyPassword(option.PrivateKey, option.PrivateKeyPassword!);
            var pkcs8PrivateKey = _rsaKey.ConvertType(decryptedKey, RsaKeyType.Pkcs8);
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(pkcs8PrivateKey.RemoveBeauty()), out var _);
        }
        catch (Exception ex)
        {
            throw new Exception("private key format error", ex);
        }
        return rsa.SignData(Encoding.UTF8.GetBytes(data), option.HashAlgorithmName, option.SignaturePadding ?? RSASignaturePadding.Pkcs1);
    }

    public bool VerifySign(string data, RsaVerifySignOption option)
    {
        if (data.IsNull()) throw new ArgumentNullException(nameof(data));
        if (option.PublicKey.IsNull()) throw new Exception("public key required");

        using var rsa = RSA.Create();
        try
        {
            var pkcs8PublicKey = _rsaKey.ConvertType(option.PublicKey, RsaKeyType.Pkcs8);
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pkcs8PublicKey.RemoveBeauty()), out var _);
        }
        catch (Exception ex)
        {
            throw new Exception("public key format error", ex);
        }
        return rsa.VerifyData(Encoding.UTF8.GetBytes(data), option.Signature, option.HashAlgorithmName, option.SignaturePadding ?? RSASignaturePadding.Pkcs1);
    }

    static void EnsureEncryptDataLengthCorrect(int keySize, byte[] data, RSAEncryptionPadding padding)
    {
        var modeSize = keySize / 8;
        var currentSize = data.Length;
        int maxSize;
        if (padding == RSAEncryptionPadding.Pkcs1) maxSize = modeSize - 11;
        else if (padding == RSAEncryptionPadding.OaepSHA1) maxSize = modeSize - 42;
        else if (padding == RSAEncryptionPadding.OaepSHA256) maxSize = modeSize - 66;
        else if (padding == RSAEncryptionPadding.OaepSHA384) maxSize = modeSize - 98;
        else if (padding == RSAEncryptionPadding.OaepSHA512) maxSize = modeSize - 130;
        else throw new NotSupportedException($"padding '{padding}' not supported");
        if (currentSize > maxSize) throw new RsaDataSizeErrorException(currentSize, maxSize);
    }
}

/// <summary>
/// rsa data size error exception
/// </summary>
public class RsaDataSizeErrorException : Exception
{
    /// <summary>
    /// rsa data size error exception
    /// </summary>
    /// <param name="currentSize">current data size</param>
    /// <param name="maxSize">max data size</param>
    public RsaDataSizeErrorException(int currentSize, int maxSize) : base($"current data lenght({currentSize}) should less than equal max length({maxSize}) ")
    {
        CurrentSize = currentSize;
        MaxSize = maxSize;
    }

    /// <summary>
    /// current data size
    /// </summary>
    public int CurrentSize { get; }
    /// <summary>
    /// max data size
    /// </summary>
    public int MaxSize { get; }
}