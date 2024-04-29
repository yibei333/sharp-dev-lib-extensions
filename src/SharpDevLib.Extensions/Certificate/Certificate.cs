using SharpDevLib.Extensions.Certificate.Opitons;
using SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SharpDevLib.Extensions.Certificate;

internal class Certificate : ICertificate
{
    public X509Certificate2 GenerateCA(CACertOption option)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.PrivateKey.RemoveBeauty()), out var _);

        var name = option.Subject.CreateX500DistinguishedName();
        var request = new CertificateRequest(option.Subject.CreateX500DistinguishedName(), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        option.Extensions.ForEach(x => request.CertificateExtensions.Add(x));

        using var caRsa = RSA.Create();
        caRsa.ImportPkcs8PrivateKey(Convert.FromBase64String((option.CAPrivateKey ?? option.PrivateKey).RemoveBeauty()), out var _);
        var serialNumbers = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        var cert = request.Create(option.CACert.IsNull() ? name : option.CACert!.IssuerName, X509SignatureGenerator.CreateForRSA(caRsa, RSASignaturePadding.Pkcs1), option.StartTime, option.EndTime, serialNumbers) ?? throw new Exception("create cert failed");
        return cert;
    }

    public X509Certificate2 GenerateClientCert(ClientCertOption option)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.PrivateKey.RemoveBeauty()), out var _);

        var request = new CertificateRequest(option.Subject.CreateX500DistinguishedName(), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        option.Extensions.ForEach(x => request.CertificateExtensions.Add(x));

        using var caRsa = RSA.Create();
        caRsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.CAPrivateKey!.RemoveBeauty()), out var _);

        var serialNumbers = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        var cert = request.Create(option.CACert!.IssuerName, X509SignatureGenerator.CreateForRSA(caRsa, RSASignaturePadding.Pkcs1), option.StartTime, option.EndTime, serialNumbers) ?? throw new Exception("create cert failed");
        return cert;
    }

    public X509Certificate2 GenerateServerCert(ServerCertOption option)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.PrivateKey.RemoveBeauty()), out var _);

        var request = new CertificateRequest(option.Subject.CreateX500DistinguishedName(), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        option.Extensions.ForEach(x => request.CertificateExtensions.Add(x));

        using var caRsa = RSA.Create();
        caRsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.CAPrivateKey!.RemoveBeauty()), out var _);

        var serialNumbers = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        var cert = request.Create(option.CACert!.IssuerName, X509SignatureGenerator.CreateForRSA(caRsa, RSASignaturePadding.Pkcs1), option.StartTime, option.EndTime, serialNumbers) ?? throw new Exception("create cert failed");
        return cert;
    }
}
