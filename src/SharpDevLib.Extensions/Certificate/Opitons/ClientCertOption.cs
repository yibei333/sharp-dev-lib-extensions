using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// client cert option
/// </summary>
public class ClientCertOption : CACertOption
{
    /// <summary>
    /// instantient client cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    public ClientCertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject) : this(caCert, caPrivateKey, privateKey, subject, DateTime.Now, DateTime.Now.AddYears(1))
    {
    }

    /// <summary>
    /// instantient client cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    /// <param name="startTime">start time</param>
    /// <param name="endTime">end time</param>
    public ClientCertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject, DateTime startTime, DateTime endTime) : base(caCert, caPrivateKey, privateKey, subject, startTime, endTime)
    {
        if (caCert.IsNull()) throw new ArgumentNullException(nameof(caCert));
        if (caPrivateKey.IsNull()) throw new ArgumentNullException(nameof(caPrivateKey));

        Extensions.Clear();
        Extensions.AddRange(new List<X509Extension>
        {
            new X509BasicConstraintsExtension(false, false, 0, false),
            new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false),
            new X509EnhancedKeyUsageExtension(new OidCollection { Oid.FromOidValue("1.3.6.1.5.5.7.3.2", OidGroup.All) }, false),
        });
    }
}
