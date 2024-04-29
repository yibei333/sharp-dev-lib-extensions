using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// server cert option
/// </summary>
public class ServerCertOption : CACertOption
{
    /// <summary>
    /// instantient server cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    /// <param name="subjectAlternativeNames">subject alternative names</param>
    public ServerCertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject, List<SubjectAlternativeNameOption> subjectAlternativeNames) : this(caCert, caPrivateKey, privateKey, subject, subjectAlternativeNames, DateTime.Now, DateTime.Now.AddYears(1))
    {
    }

    /// <summary>
    /// instantient server cert option
    /// </summary>
    /// <param name="caCert">ca cert</param>
    /// <param name="caPrivateKey">ca private key</param>
    /// <param name="privateKey">pkcs8 format private key</param>
    /// <param name="subject">subject info</param>
    /// <param name="subjectAlternativeNames">subject alternative names</param>
    /// <param name="startTime">start time</param>
    /// <param name="endTime">end time</param>
    public ServerCertOption(X509Certificate2 caCert, string caPrivateKey, string privateKey, SubjectOption subject, List<SubjectAlternativeNameOption> subjectAlternativeNames, DateTime startTime, DateTime endTime) : base(caCert, caPrivateKey, privateKey, subject, startTime, endTime)
    {
        if (caCert.IsNull()) throw new ArgumentNullException(nameof(caCert));
        if (caPrivateKey.IsNull()) throw new ArgumentNullException(nameof(caPrivateKey));

        SubjectAlternativeNames = subjectAlternativeNames;
        if (SubjectAlternativeNames.IsEmpty()) throw new ArgumentException($"argument '{nameof(subjectAlternativeNames)}' can not be empty");

        var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();
        foreach (var item in SubjectAlternativeNames)
        {
            if (item.Type == SubjectAlternativeNameType.Uri) subjectAlternativeNameBuilder.AddUri(new Uri(item.Value));
            else if (item.Type == SubjectAlternativeNameType.Email) subjectAlternativeNameBuilder.AddEmailAddress(item.Value);
            else if (item.Type == SubjectAlternativeNameType.UPN) subjectAlternativeNameBuilder.AddUserPrincipalName(item.Value);
            else if (item.Type == SubjectAlternativeNameType.Dns) subjectAlternativeNameBuilder.AddDnsName(item.Value);
            else if (item.Type == SubjectAlternativeNameType.IP) subjectAlternativeNameBuilder.AddIpAddress(IPAddress.Parse(item.Value));
            else throw new NotSupportedException($"subject alternative name type [{item.Type}] not supported");
        }
        var subjectAlternativeNameExtension = subjectAlternativeNameBuilder.Build();

        Extensions.Clear();
        Extensions.AddRange(new List<X509Extension>
        {
            new X509BasicConstraintsExtension(false, false, 0, false),
            new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false),
            new X509EnhancedKeyUsageExtension(new OidCollection { Oid.FromOidValue("1.3.6.1.5.5.7.3.1", OidGroup.All) }, false),
            subjectAlternativeNameExtension
        });
    }

    /// <summary>
    /// subject alternative names
    /// </summary>
    public List<SubjectAlternativeNameOption> SubjectAlternativeNames { get; }
}

/// <summary>
/// subject alternative name type
/// </summary>
public enum SubjectAlternativeNameType
{
    /// <summary>
    /// dns
    /// </summary>
    Dns,
    /// <summary>
    /// ip address
    /// </summary>
    IP,
    /// <summary>
    /// uri
    /// </summary>
    Uri,
    /// <summary>
    /// email
    /// </summary>
    Email,
    /// <summary>
    /// user pricipal name
    /// </summary>
    UPN
}

/// <summary>
/// subject alternative name option
/// </summary>
public class SubjectAlternativeNameOption
{
    /// <summary>
    /// instantient subject alternative name option
    /// </summary>
    /// <param name="type">subject alternative name type</param>
    /// <param name="value">subject alternative value</param>
    public SubjectAlternativeNameOption(SubjectAlternativeNameType type, string value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// subject alternative name type
    /// </summary>
    public SubjectAlternativeNameType Type { get; }

    /// <summary>
    /// subject alternative value
    /// </summary>
    public string Value { get; }
}