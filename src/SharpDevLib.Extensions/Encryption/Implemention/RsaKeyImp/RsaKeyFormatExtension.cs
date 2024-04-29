using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;

internal static class RsaKeyFormatExtension
{
    private static readonly Dictionary<string, RsaKeyFormat> _keyFormatMapping = new()
    {
        { RsaKeyFormatConst.XmlStart, RsaKeyFormat.XmlPrivate },
        { RsaKeyFormatConst.Pkcs1PrivateStart, RsaKeyFormat.Pkcs1Private },
        { RsaKeyFormatConst.Pkcs8PrivateStart, RsaKeyFormat.Pkcs8Private },
        { RsaKeyFormatConst.Pkcs8EncryptedPrivateStart, RsaKeyFormat.Pkcs8EncryptedPrivate },
        { RsaKeyFormatConst.PemPublicStart, RsaKeyFormat.PemPublic },
    };

    private static readonly Dictionary<RsaKeyFormat, KeyValuePair<string, string>> _startEndMappding = new()
    {
        { RsaKeyFormat.Pkcs1Private, new KeyValuePair<string, string>(RsaKeyFormatConst.Pkcs1PrivateStart, RsaKeyFormatConst.Pkcs1PrivateEnd) },
        { RsaKeyFormat.Pkcs8Private, new KeyValuePair<string, string>(RsaKeyFormatConst.Pkcs8PrivateStart, RsaKeyFormatConst.Pkcs8PrivateEnd) },
        { RsaKeyFormat.Pkcs8EncryptedPrivate, new KeyValuePair<string, string>(RsaKeyFormatConst.Pkcs8EncryptedPrivateStart, RsaKeyFormatConst.Pkcs8EncryptedPrivateEnd) },
        { RsaKeyFormat.PemPublic, new KeyValuePair<string, string>(RsaKeyFormatConst.PemPublicStart, RsaKeyFormatConst.PemPublicEnd) },
    };

    public static RsaKeyType GetRsaKeyType(this string? key)
    {
        if (key.IsNull()) return RsaKeyType.UnKnown;
        var format = key.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.XmlPrivate or RsaKeyFormat.XmlPublic => RsaKeyType.Xml,
            RsaKeyFormat.Pkcs1Private or RsaKeyFormat.Pkcs1EncryptedPrivate => RsaKeyType.Pkcs1,
            RsaKeyFormat.Pkcs8Private or RsaKeyFormat.Pkcs8EncryptedPrivate => RsaKeyType.Pkcs8,
            RsaKeyFormat.PemPublic => RsaKeyType.PemPublic,
            _ => RsaKeyType.UnKnown,
        };
    }

    public static RsaKeyFormat GetRsaKeyFormat(this string? key)
    {
        if (key.IsNull()) return RsaKeyFormat.UnKnown;
        var trimKey = key!.Trim();
        var mapping = _keyFormatMapping.FirstOrDefault(x => trimKey.StartsWith(x.Key));
        if (mapping.IsNull()) return RsaKeyFormat.UnKnown;
        var format = mapping.Value;
        if (format == RsaKeyFormat.XmlPrivate)
        {
            var parameters = key.ToParameter();
            format = parameters.InverseQ.IsEmpty() ? RsaKeyFormat.XmlPublic : RsaKeyFormat.XmlPrivate;
        }
        else if (format == RsaKeyFormat.Pkcs1Private)
        {
            format = key.Contains("DEK-Info") ? RsaKeyFormat.Pkcs1EncryptedPrivate : RsaKeyFormat.Pkcs1Private;
        }
        return format;
    }

    public static string Beauty(this string key)
    {
        key = key?.Trim() ?? string.Empty;
        var format = key.GetRsaKeyFormat();
        if (format == RsaKeyFormat.Pkcs1EncryptedPrivate) return key.Trim();
        var startEnd = _startEndMappding.ContainsKey(format) ? _startEndMappding[format] : new KeyValuePair<string, string>("", "");
        var body = key.TrimStart(startEnd.Key).TrimEnd(startEnd.Value).Trim().Replace("\r", "").Replace("\n", "").Replace("\r\n", "");

        var builder = new StringBuilder();
        builder.AppendLine(startEnd.Key);

        var count = body.Length / 64;
        for (int i = 0; i < count; i++)
        {
            builder.AppendLine(body.Substring(i * 64, 64));
        }

        var remainder = body.Length % 64;
        if (remainder > 0) builder.AppendLine(body.Substring(count * 64, remainder));

        builder.AppendLine(startEnd.Value);
        return builder.ToString().Trim();
    }

    public static string RemoveBeauty(this string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var format = key.Trim().GetRsaKeyFormat();
        if (format == RsaKeyFormat.Pkcs1EncryptedPrivate) return key.Trim();
        var startEnd = _startEndMappding.ContainsKey(format) ? _startEndMappding[format] : new KeyValuePair<string, string>("", "");

        var body = key.Trim().TrimStart(startEnd.Key).TrimEnd(startEnd.Value).Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\r\n", "");
        return body;
    }

    public static string ToXml(this RSAParameters parameters, bool isPrivate)
    {
        if (!isPrivate)
        {
            var pmoduleStr = Convert.ToBase64String(parameters.Modulus!);
            var pexponentStr = Convert.ToBase64String(parameters.Exponent!);
            var publicXml = new XElement("RSAKeyValue");
            publicXml.Add(new XElement("Modulus", pmoduleStr));
            publicXml.Add(new XElement("Exponent", pexponentStr));
            return publicXml.ToString();
        }

        var dStr = Convert.ToBase64String(parameters.D!);
        var dpStr = Convert.ToBase64String(parameters.DP!);
        var dqStr = Convert.ToBase64String(parameters.DQ!);
        var moduleStr = Convert.ToBase64String(parameters.Modulus!);
        var exponentStr = Convert.ToBase64String(parameters.Exponent!);
        var inverseQStr = Convert.ToBase64String(parameters.InverseQ!);
        var pStr = Convert.ToBase64String(parameters.P!);
        var qStr = Convert.ToBase64String(parameters.Q!);

        var privateXml = new XElement("RSAKeyValue");
        privateXml.Add(new XElement("DP", dpStr));
        privateXml.Add(new XElement("DQ", dqStr));
        privateXml.Add(new XElement("D", dStr));
        privateXml.Add(new XElement("Modulus", moduleStr));
        privateXml.Add(new XElement("Exponent", exponentStr));
        privateXml.Add(new XElement("InverseQ", inverseQStr));
        privateXml.Add(new XElement("P", pStr));
        privateXml.Add(new XElement("Q", qStr));
        return privateXml.ToString();
    }

    public static RSAParameters ToParameter(this string xmlKey)
    {
        if (xmlKey.IsNull()) throw new ArgumentNullException(nameof(xmlKey));
        xmlKey = xmlKey.Trim();
        var result = new RSAParameters();
        var elements = XDocument.Parse(xmlKey).Elements().First().Elements().ToList();
        foreach (var element in elements)
        {
            switch (element.Name.ToString())
            {
                case "DP":
                    result.DP = Convert.FromBase64String(element.Value);
                    break;
                case "DQ":
                    result.DQ = Convert.FromBase64String(element.Value);
                    break;
                case "D":
                    result.D = Convert.FromBase64String(element.Value);
                    break;
                case "Modulus":
                    result.Modulus = Convert.FromBase64String(element.Value);
                    break;
                case "Exponent":
                    result.Exponent = Convert.FromBase64String(element.Value);
                    break;
                case "InverseQ":
                    result.InverseQ = Convert.FromBase64String(element.Value);
                    break;
                case "P":
                    result.P = Convert.FromBase64String(element.Value);
                    break;
                case "Q":
                    result.Q = Convert.FromBase64String(element.Value);
                    break;
                default:
                    break;
            }
        }
        return result;
    }

    public static bool IsPrivate(this string key)
    {
        if (key.IsNull()) throw new ArgumentNullException(nameof(key));
        key = key.Trim();
        var format = key.GetRsaKeyFormat();
        return format switch
        {
            RsaKeyFormat.XmlPrivate or RsaKeyFormat.Pkcs1Private or RsaKeyFormat.Pkcs1EncryptedPrivate or RsaKeyFormat.Pkcs8Private or RsaKeyFormat.Pkcs8EncryptedPrivate => true,
            RsaKeyFormat.PemPublic or RsaKeyFormat.XmlPublic => false,
            _ => throw new NotImplementedException(),
        };
    }
}