using Newtonsoft.Json.Linq;
using SharpDevLib.Extensions.Encryption.Implemention.RsaKeyImp;
using System.Security.Cryptography;
using System.Text;

namespace SharpDevLib.Extensions.Jwt;

internal class JwtService : IJwtService
{
    public string Create(JwtCreateOption option)
    {
        var header = CreateHeader(option);
        var payload = CreatePayload(option);
        var signature = CreateSignature(header, payload, option);
        return $"{header}.{payload}.{signature}";
    }

    public JwtVerifyResult Verify(JwtVerifyOption option)
    {
        if (option.Token.IsEmpty()) return new JwtVerifyResult(false);
        var str = option.Token.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (str.Length != 3) return new JwtVerifyResult(false);

        var headerSegment = str[0];
        var payloadSegment = str[1];
        var signatureSegment = str[2];

        var headerObject = headerSegment.Base64UrlDecode().DeSerialize<JObject>();
        var algorithmString = headerObject?.GetValue("alg")?.ToString() ?? string.Empty;
        var algorithm = Enum.Parse<JwtAlgorithm>(algorithmString);
        if (!algorithm.IsEnum<JwtAlgorithm>()) throw new NotSupportedException($"{algorithm} not supported");

        if (algorithm == JwtAlgorithm.HS256)
        {
            var signatureToVerify = $"{headerSegment}.{payloadSegment}".HMACSHA256Hash(option.Key).FromHexString().Base64UrlEncode();
            var verified = signatureToVerify == signatureSegment;
            return verified ? new JwtVerifyResult(true, algorithm, headerSegment.Base64UrlDecode(), payloadSegment.Base64UrlDecode(), signatureSegment) : new JwtVerifyResult(false);
        }

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(option.Key.RemoveBeauty()), out var _);
        var isVerified = rsa.VerifyData(Encoding.UTF8.GetBytes($"{headerSegment}.{payloadSegment}"), signatureSegment.Base64UrlDecodeBytes(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return isVerified ? new JwtVerifyResult(true, algorithm, headerSegment.Base64UrlDecode(), payloadSegment.Base64UrlDecode(), signatureSegment) : new JwtVerifyResult(false);
    }

    private static string CreateHeader(JwtCreateOption option)
    {
        if (!option.Algorithm.IsEnum<JwtAlgorithm>()) throw new NotSupportedException($"{option.Algorithm} not supported");
        var header = new { alg = option.Algorithm.ToString(), typ = "JWT" };
        var headerSegment = header.Serialize().Base64UrlEncode();
        return headerSegment;
    }

    private static string CreatePayload(JwtCreateOption option)
    {
        if (option.Payload.IsNull()) throw new ArgumentNullException(nameof(option.Payload));
        return option.Payload.Serialize().Base64UrlEncode();
    }

    private static string CreateSignature(string header, string payload, JwtCreateOption option)
    {
        if (option.Algorithm == JwtAlgorithm.HS256) return $"{header}.{payload}".HMACSHA256Hash(option.Key).FromHexString().Base64UrlEncode();

        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(option.Key.RemoveBeauty()), out var _);
        var signature = rsa.SignData(Encoding.UTF8.GetBytes($"{header}.{payload}"), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).Base64UrlEncode();
        return signature;
    }
}
