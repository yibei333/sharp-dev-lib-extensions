using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;

internal class RsaKey : IRsaKey
{
    public string ConvertType(string key, RsaKeyType targetType)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        if (IsKeyHasPassword(key)) throw new Exception("please remove password first");
        key = key.Trim();
        var type = key.GetRsaKeyType();
        if (type == RsaKeyType.UnKnown || type == targetType || targetType == RsaKeyType.UnKnown) return key;

        var isPrivate = key.IsPrivate();
        return type switch
        {
            RsaKeyType.Xml => targetType == RsaKeyType.Pkcs1 ? (isPrivate ? XmlPrivateToPkcs1(key) : XmlPublicToPem(key)) : (isPrivate ? XmlPrivateToPkcs8(key) : XmlPublicToPem(key)),
            RsaKeyType.Pkcs1 => targetType == RsaKeyType.Xml ? Pkcs1PrivateToXml(key) : Pkcs1PrivateToPkcs8(key),
            RsaKeyType.Pkcs8 => targetType == RsaKeyType.Xml ? Pkcs8PrivateToXml(key) : Pkcs8PrivateToPkcs1(key),
            RsaKeyType.PemPublic => targetType == RsaKeyType.Xml ? PemPublicToXml(key) : key,
            _ => throw new NotSupportedException(),
        };
    }

    public string ExportPrivateKey(RsaKeyType type, int? size = 2048)
    {
        return type switch
        {
            RsaKeyType.Xml => ExportXmlPrivateKey(size),
            RsaKeyType.Pkcs1 => ExportPkcs1PrivateKey(size),
            RsaKeyType.Pkcs8 => ExportPkcs8PrivateKey(size),
            _ => throw new NotSupportedException(),
        };
    }

    public string ExportPublicKey(string privateKey, string? password = null)
    {
        var format = privateKey.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.XmlPrivate => ExportXmlPublicKey(privateKey),
            RsaKeyFormat.Pkcs1Private => ExportPkcs1PublicKey(privateKey),
            RsaKeyFormat.Pkcs1EncryptedPrivate => ExportPkcs1PublicKey(privateKey, password),
            RsaKeyFormat.Pkcs8Private => ExportPkcs8PublicKey(privateKey),
            RsaKeyFormat.Pkcs8EncryptedPrivate => ExportPkcs8PublicKey(privateKey, password),
            _ => throw new NotSupportedException(),
        };
    }

    public string Format(string key, RsaKeyFormat format)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        return format switch
        {
            RsaKeyFormat.Pkcs1Private => FormatPkcs1Private(key),
            RsaKeyFormat.Pkcs8Private => FormatPkcs8Private(key),
            RsaKeyFormat.Pkcs8EncryptedPrivate => FormatEncryptedPkcs8Private(key),
            RsaKeyFormat.PemPublic => FormatPemPublic(key),
            _ => key
        };
    }

    public string AddPrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        var format = privateKey.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.Pkcs1Private => AddPkcs1PrivateKeyPassword(privateKey, password),
            RsaKeyFormat.Pkcs8Private => AddPkcs8PrivateKeyPassword(privateKey, password),
            _ => throw new NotSupportedException(),
        };
    }

    public string RemovePrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        var format = privateKey.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.Pkcs1EncryptedPrivate => RemovePkcs1PrivateKeyPassword(privateKey, password),
            RsaKeyFormat.Pkcs8EncryptedPrivate => RemovePkcs8PrivateKeyPassword(privateKey, password),
            _ => throw new NotSupportedException(),
        };
    }

    public string UpdatePrivateKeyPassword(string privateKey, string oldPassword, string newPassword)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        var format = privateKey.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.Pkcs1EncryptedPrivate => UpdatePkcs1PrivateKeyPassword(privateKey, oldPassword, newPassword),
            RsaKeyFormat.Pkcs8EncryptedPrivate => UpdatePkcs8PrivateKeyPassword(privateKey, oldPassword, newPassword),
            _ => throw new NotSupportedException(),
        };
    }

    public string RemoveFormat(string key) => key.RemoveBeauty();

    public bool IsKeyPairMatch(string privateKey, string publicKey, string? password=null)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        var keyType = privateKey.GetRsaKeyType();
        var expectedPublic = keyType switch
        {
            RsaKeyType.Xml => ExportXmlPublicKey(privateKey),
            RsaKeyType.Pkcs1 => ExportPkcs1PublicKey(privateKey, password),
            RsaKeyType.Pkcs8 => ExportPkcs8PublicKey(privateKey, password),
            _ => string.Empty
        };
        return expectedPublic.Beauty() == publicKey.Beauty();
    }

    public bool IsKeyHasPassword(string key)
    {
        if (!IsPrivate(key)) return false;
        if (key.Contains(RsaKeyFormatConst.Pkcs8EncryptedPrivateStart) || key.Contains("DEK-Info:")) return true;
        return false;
    }

    #region Private
    private static string AddPkcs1PrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        if (password.IsNull()) throw new ArgumentNullException(nameof(password));

        using var strReader = new StringReader(privateKey);
        var pemReader = new PemReader(strReader);
        var key = pemReader.ReadObject() as AsymmetricCipherKeyPair;
        if (key.IsNull()) throw new Exception($"read key failed");

        using var strWriter = new StringWriter();
        var pemWriter = new PemWriter(strWriter);
        pemWriter.WriteObject(key!.Private, "AES-256-CBC", password.ToCharArray(), new SecureRandom());
        return strWriter.ToString().Trim();
    }

    private static string AddPkcs8PrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportEncryptedPkcs8PrivateKey(Encoding.UTF8.GetBytes(password), new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, rsa.KeySize));
        return FormatEncryptedPkcs8Private(Convert.ToBase64String(bytes));
    }

    private static string ExportPkcs1PrivateKey(int? size = 2048)
    {
        using var rsa = RSA.Create();
        rsa.KeySize = size ?? 2048;
        var bytes = rsa.ExportRSAPrivateKey();
        return FormatPkcs1Private(Convert.ToBase64String(bytes));
    }

    private string ExportPkcs1PublicKey(string privateKey, string? password = null)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        var hasPassword = IsKeyHasPassword(privateKey);
        if (hasPassword && password.IsNull()) throw new ArgumentNullException(nameof(password));
        if (hasPassword) privateKey = RemovePkcs1PrivateKeyPassword(privateKey, password!);
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportSubjectPublicKeyInfo();
        return FormatPemPublic(Convert.ToBase64String(bytes));
    }

    private static string ExportPkcs8PrivateKey(int? size = 2048)
    {
        using var rsa = RSA.Create();
        rsa.KeySize = size ?? 2048;
        var bytes = rsa.ExportPkcs8PrivateKey();
        return FormatPkcs8Private(Convert.ToBase64String(bytes));
    }

    private string ExportPkcs8PublicKey(string privateKey, string? password = null)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        var hasPassword = IsKeyHasPassword(privateKey);
        if (hasPassword && password.IsNull()) throw new ArgumentNullException(nameof(password));
        if (hasPassword) privateKey = RemovePkcs8PrivateKeyPassword(privateKey, password!);
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportSubjectPublicKeyInfo();
        return FormatPemPublic(Convert.ToBase64String(bytes));
    }

    private static string ExportXmlPrivateKey(int? size = 2048)
    {
        using var rsa = RSA.Create();
        rsa.KeySize = size ?? 2048;
        var parameters = rsa.ExportParameters(true);
        return parameters.ToXml(true);
    }

    private static string ExportXmlPublicKey(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        var parameters = privateKey.ToParameter();
        return parameters.ToXml(false);
    }

    private static string FormatPkcs1Private(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var unformatedKey = key.RemoveBeauty();
        return $"{RsaKeyFormatConst.Pkcs1PrivateStart}\r\n{unformatedKey.Beauty()}\r\n{RsaKeyFormatConst.Pkcs1PrivateEnd}";
    }

    private static string FormatPemPublic(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var unformatedKey = key.RemoveBeauty();
        return $"{RsaKeyFormatConst.PemPublicStart}\r\n{unformatedKey.Beauty()}\r\n{RsaKeyFormatConst.PemPublicEnd}";
    }

    private static string FormatPkcs8Private(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var unformatedKey = key.RemoveBeauty();
        return $"{RsaKeyFormatConst.Pkcs8PrivateStart}\r\n{unformatedKey.Beauty()}\r\n{RsaKeyFormatConst.Pkcs8PrivateEnd}";
    }

    private static string FormatEncryptedPkcs8Private(string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var unformatedKey = key.RemoveBeauty();
        return $"{RsaKeyFormatConst.Pkcs8EncryptedPrivateStart}\r\n{unformatedKey.Beauty()}\r\n{RsaKeyFormatConst.Pkcs8EncryptedPrivateEnd}";
    }

    private static string Pkcs1PrivateToPkcs8(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportPkcs8PrivateKey();
        return FormatPkcs8Private(Convert.ToBase64String(bytes));
    }

    private static string Pkcs1PrivateToXml(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var parameters = rsa.ExportParameters(true);
        return parameters.ToXml(true);
    }

    private static string PemPublicToXml(string publicKey)
    {
        if (publicKey.IsNull()) throw new ArgumentNullException(nameof(publicKey));
        publicKey = publicKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey.RemoveBeauty()), out var _);
        var parameters = rsa.ExportParameters(false);
        return parameters.ToXml(false);
    }

    private static string Pkcs8PrivateToPkcs1(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportRSAPrivateKey();
        return FormatPkcs1Private(Convert.ToBase64String(bytes));
    }

    private static string Pkcs8PrivateToXml(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var parameters = rsa.ExportParameters(true);
        return parameters.ToXml(true);
    }

    private static string XmlPrivateToPkcs1(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        var parameters = privateKey.ToParameter();
        rsa.ImportParameters(parameters);
        var bytes = rsa.ExportRSAPrivateKey();
        return FormatPkcs1Private(Convert.ToBase64String(bytes));
    }

    private static string XmlPrivateToPkcs8(string privateKey)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        using var rsa = RSA.Create();
        var parameters = privateKey.ToParameter();
        rsa.ImportParameters(parameters);
        var bytes = rsa.ExportPkcs8PrivateKey();
        return FormatPkcs8Private(Convert.ToBase64String(bytes));
    }

    private static string XmlPublicToPem(string publicKey)
    {
        if (publicKey.IsNull()) throw new ArgumentNullException(nameof(publicKey));
        publicKey = publicKey.Trim();
        using var rsa = RSA.Create();
        var parameters = publicKey.ToParameter();
        rsa.ImportParameters(parameters);
        var bytes = rsa.ExportSubjectPublicKeyInfo();
        return FormatPemPublic(Convert.ToBase64String(bytes));
    }

    private static string RemovePkcs1PrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        if (password.IsNull()) throw new ArgumentNullException(nameof(password));

        using var strReader = new StringReader(privateKey);
        var pemReader = new PemReader(strReader, new RsaPassword(password.ToArray()));
        var key = pemReader.ReadObject() as AsymmetricCipherKeyPair;
        if (key.IsNull()) throw new Exception($"read key failed");

        using var strWriter = new StringWriter();
        var pemWriter = new PemWriter(strWriter);
        pemWriter.WriteObject(key!.Private);
        return FormatPkcs1Private(strWriter.ToString());
    }

    private static string RemovePkcs8PrivateKeyPassword(string privateKey, string password)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        privateKey = privateKey.Trim();
        if (password.IsNull()) throw new ArgumentNullException(nameof(password));
        using var rsa = RSA.Create();
        //rsa.ImportFromEncryptedPem(privateKey, Encoding.UTF8.GetBytes(password));
        rsa.ImportEncryptedPkcs8PrivateKey(Encoding.UTF8.GetBytes(password), Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var bytes = rsa.ExportPkcs8PrivateKey();
        return FormatPkcs8Private(Convert.ToBase64String(bytes));
    }

    private static string UpdatePkcs1PrivateKeyPassword(string privateKey, string oldPassword, string newPassword)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        if (oldPassword.IsNull()) throw new ArgumentNullException(nameof(oldPassword));
        if (newPassword.IsNull()) throw new ArgumentNullException(nameof(newPassword));
        privateKey = privateKey.Trim();
        var removedKey = RemovePkcs1PrivateKeyPassword(privateKey, oldPassword);
        return AddPkcs1PrivateKeyPassword(removedKey, newPassword);
    }

    private static string UpdatePkcs8PrivateKeyPassword(string privateKey, string oldPassword, string newPassword)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        if (oldPassword.IsNull()) throw new ArgumentNullException(nameof(oldPassword));
        if (newPassword.IsNull()) throw new ArgumentNullException(nameof(newPassword));
        privateKey = privateKey.Trim();
        var removedKey = RemovePkcs8PrivateKeyPassword(privateKey, oldPassword);
        return AddPkcs8PrivateKeyPassword(removedKey, newPassword);
    }

    public RsaKeyType KeyType(string key) => key.GetRsaKeyType();

    public RsaKeyFormat KeyFormat(string key) => key.GetRsaKeyFormat();

    public bool IsPrivate(string key) => key.IsPrivate();

    public RSAParameters ToParameter(string xmlKey) => xmlKey.ToParameter();

    public string ToXml(RSAParameters parameters) => parameters.ToXml(parameters.InverseQ.NotEmpty());
    #endregion
}
