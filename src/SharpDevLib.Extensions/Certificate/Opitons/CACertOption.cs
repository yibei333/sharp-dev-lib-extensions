using SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// ca cert option
/// </summary>
public class CACertOption
{
    /// <summary>
    /// instantient ca cert option
    /// </summary>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    public CACertOption(string privateKey, SubjectOption subject) : this(privateKey, subject, DateTime.Now, DateTime.Now.AddYears(1))
    {
    }

    /// <summary>
    /// instantient ca cert option
    /// </summary>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    /// <param name="startTime">start time</param>
    /// <param name="endTime">end time</param>
    public CACertOption(string privateKey, SubjectOption subject, DateTime startTime, DateTime endTime)
    {
        if (privateKey.IsNull()) throw new ArgumentNullException(nameof(privateKey));
        if (subject.IsNull()) throw new ArgumentNullException(nameof(subject));

        PrivateKey = privateKey;
        Subject = subject;

        var subjectKeyIdentifierExtension = CreateSubjectKeyIdentifierExtension(privateKey);
        Extensions = new List<X509Extension>
        {
            new X509BasicConstraintsExtension(true, false, 0, false),
            new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, false),
            new X509EnhancedKeyUsageExtension(new OidCollection {
                //Oid.FromFriendlyName("Client Authentication", OidGroup.EnhancedKeyUsage),//系统语言不同，友好名称不同，不能这样做
                //https://www.pkisolutions.com/object-identifiers-oid-in-pki/
                Oid.FromOidValue("1.3.6.1.5.5.7.3.2", OidGroup.All),//Client Authentication
                Oid.FromOidValue("1.3.6.1.5.5.7.3.1", OidGroup.All),//Server Authentication
                Oid.FromOidValue("1.3.6.1.5.5.7.3.3", OidGroup.All),//Code Signing
                Oid.FromOidValue("1.3.6.1.4.1.311.10.3.4", OidGroup.All),//Encrypting File System
                Oid.FromOidValue("1.3.6.1.5.5.7.3.4", OidGroup.All),//Secure Email
                Oid.FromOidValue("1.3.6.1.5.5.7.3.7", OidGroup.All),//IP security user
                Oid.FromOidValue("1.3.6.1.5.5.7.3.6", OidGroup.All),//IP security tunnel termination
                Oid.FromOidValue("1.3.6.1.5.5.7.3.8", OidGroup.All),//Time Stamping
            }, false),
            subjectKeyIdentifierExtension,
            new X509AuthorityKeyIdentifierExtension(subjectKeyIdentifierExtension.RawData,false)
        };
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// instantient ca cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    public CACertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject) : this(caCert, caPrivateKey, privateKey, subject, DateTime.Now, DateTime.Now.AddYears(1))
    {
    }

    /// <summary>
    /// instantient ca cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    /// <param name="startTime">start time</param>
    /// <param name="endTime">end time</param>
    public CACertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject, DateTime startTime, DateTime endTime) : this(privateKey, subject, startTime, endTime)
    {
        if ((caCert.IsNull() && caPrivateKey.NotNull()) || (caCert.NotNull() && caPrivateKey.IsNull())) throw new ArgumentException($"ca and cakey is a pair");

        CACert = caCert;
        CAPrivateKey = caPrivateKey;
    }

    /// <summary>
    /// pkcs8 format private key
    /// </summary>
    public string PrivateKey { get; }
    /// <summary>
    /// subject info
    /// </summary>
    public SubjectOption Subject { get; }
    /// <summary>
    /// extensions
    /// </summary>
    public List<X509Extension> Extensions { get; }
    /// <summary>
    /// start time
    /// </summary>
    public DateTime StartTime { get; }
    /// <summary>
    /// end time
    /// </summary>
    public DateTime EndTime { get; }

    /// <summary>
    /// ca cert
    /// </summary>
    public X509Certificate2? CACert { get; }

    /// <summary>
    /// ca private key
    /// </summary>
    public string? CAPrivateKey { get; }


    private X509SubjectKeyIdentifierExtension CreateSubjectKeyIdentifierExtension(string privateKey)
    {
        var rsaOid = "1.2.840.113549.1.1.1";// found under System.Security.Cryptography.CngLightup.RsaOid but it's marked as private
        var oid = new Oid(rsaOid);
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey.RemoveBeauty()), out var _);
        var keyValue = new AsnEncodedData(rsa.ExportSubjectPublicKeyInfo());
        var keyParam = new AsnEncodedData(new byte[] { 05, 00 });// ASN.1 code for NULL
        var publicKey = new PublicKey(oid, keyParam, keyValue);
        return new X509SubjectKeyIdentifierExtension(publicKey, false);
    }
}

internal class X509AuthorityKeyIdentifierExtension : X509Extension
{
    private static Oid AuthorityKeyIdentifierOid => new("2.5.29.35");

    public X509AuthorityKeyIdentifierExtension(byte[] subjectKeyIdentifierRawData, bool critical) : base(AuthorityKeyIdentifierOid, EncodeExtension(subjectKeyIdentifierRawData), critical)
    {
    }

    private static byte[] EncodeExtension(byte[] subjectKeyIdentifierRawData)
    {
        var segment = new ArraySegment<byte>(subjectKeyIdentifierRawData, 2, subjectKeyIdentifierRawData.Length - 2);
        var authorityKeyIdentifier = new byte[segment.Count + 4];
        // KeyID of the AuthorityKeyIdentifier
        authorityKeyIdentifier[0] = 0x30;
        authorityKeyIdentifier[1] = 0x16;
        authorityKeyIdentifier[2] = 0x80;
        authorityKeyIdentifier[3] = 0x14;
        segment.CopyTo(authorityKeyIdentifier, 4);
        return authorityKeyIdentifier;
    }
}