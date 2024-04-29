using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SharpDevLib.Extensions.Certificate;

/// <summary>
/// certificate service di extension
/// </summary>
public static class CertificateExtension
{
    /// <summary>
    /// add certificate service
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddCertificate(this IServiceCollection services)
    {
        services.AddTransient<ICertificate, Certificate>();
        return services;
    }

    /// <summary>
    /// save cert to file
    /// </summary>
    /// <param name="certificate">cert</param>
    /// <param name="filePath">file path</param>
    public static void SaveCrt(this X509Certificate2 certificate, string filePath)
    {
        new FileInfo(filePath).DirectoryName.EnsureDirectoryExist();
        File.WriteAllText(filePath, certificate.Export(X509ContentType.Cert).Base64Encode().BeautyCrt());
    }

    //private static byte[] GetCrtRawData(this string cert)
    //{
    //    var raw = cert.RemoveCrtBeauty();
    //    return Convert.FromBase64String(raw);
    //}

    private static readonly KeyValuePair<string, string> _crtStartEnd = new("-----BEGIN CERTIFICATE-----", "-----END CERTIFICATE-----");

    private static string BeautyCrt(this string cert)
    {
        var body = cert.Trim().TrimStart(_crtStartEnd.Key).TrimEnd(_crtStartEnd.Value).Trim().Replace("\r", "").Replace("\n", "").Replace("\r\n", "");

        var builder = new StringBuilder();
        builder.AppendLine(_crtStartEnd.Key);

        var count = body.Length / 64;
        for (int i = 0; i < count; i++)
        {
            builder.AppendLine(body.Substring(i * 64, 64));
        }

        var remainder = body.Length % 64;
        if (remainder > 0) builder.AppendLine(body.Substring(count * 64, remainder));

        builder.AppendLine(_crtStartEnd.Value);
        return builder.ToString().Trim();
    }
}