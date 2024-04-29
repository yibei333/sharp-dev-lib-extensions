//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Org.BouncyCastle.OpenSsl;
//using SharpDevLib.Extensions.Encryption;
//using System;
//using System.Formats.Asn1;
//using System.IO;
//using System.Security.Cryptography;
//using System.Text;
//using XC.RSAUtil;

//namespace SharpDevLib.Extensions.Tests.Encryption;

//[TestClass]
//public class EncryptionTests
//{
//    private readonly IEncryption _encryption;
//    private readonly IRsaKey _rsaKey;
//    public EncryptionTests()
//    {
//        IServiceCollection services = new ServiceCollection();
//        services.AddEncryption();
//        var provider = services.BuildServiceProvider();
//        _encryption = provider.GetRequiredService<IEncryption>();
//        _rsaKey = provider.GetRequiredService<IRsaKey>();
//    }

//    #region AES
//    [TestMethod]
//    [DataRow("qATp2sIMat4Calvg8z+eag==", "Hello,World", "123", null, null, null)]
//    [DataRow("s7sOdhY8rtP/MXd9CRnG/w==", "Hello,World", "123", null, CipherMode.ECB, null)]
//    [DataRow("S/KJJqLpnBZdXrqx21aigw==", "Hello,World", "123", "1111111111111111", null, null)]
//    public void AesEncryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new AesEncryptOption(key, _encryption.Symmetric.Aes.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.Aes.Encrypt(data, option);
//        var actual = Convert.ToBase64String(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void AesEncryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile.txt";
//        var targetPath = "TestFile_AESEncrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.Aes.EncryptFile(sourcePath, new AesEncryptOption("123", _encryption.Symmetric.Aes.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }

//    [TestMethod]
//    [DataRow("Hello,World", "qATp2sIMat4Calvg8z+eag==", "123", null, null, null)]
//    [DataRow("Hello,World", "s7sOdhY8rtP/MXd9CRnG/w==", "123", null, CipherMode.ECB, null)]
//    [DataRow("Hello,World", "S/KJJqLpnBZdXrqx21aigw==", "123", "1111111111111111", null, null)]
//    public void AesDecryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new AesDecryptOption(key, _encryption.Symmetric.Aes.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.Aes.Decrypt(data, option);
//        var actual = Encoding.UTF8.GetString(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void AesDecryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile_AESEncrypted.txt";
//        var targetPath = "TestFile_AESDecrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.Aes.DecryptFile(sourcePath, new AesDecryptOption("123", _encryption.Symmetric.Aes.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }
//    #endregion

//    #region Des
//    [TestMethod]
//    [DataRow("GP4715XVDrNb7WB/mArJcQ==", "Hello,World", "123", null, null, null)]
//    [DataRow("FgV3tiq+guuwfn6O9xvcBQ==", "Hello,World", "123", null, CipherMode.ECB, null)]
//    [DataRow("ZSAlymayVxGI5pcdzs2GBA==", "Hello,World", "123", "1111111111111111", null, null)]
//    public void DesEncryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new DesEncryptOption(key, _encryption.Symmetric.Des.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.Des.Encrypt(data, option);
//        var actual = Convert.ToBase64String(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void DesEncryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile.txt";
//        var targetPath = "TestFile_DesEncrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.Des.EncryptFile(sourcePath, new DesEncryptOption("123", _encryption.Symmetric.Des.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }

//    [TestMethod]
//    [DataRow("Hello,World", "GP4715XVDrNb7WB/mArJcQ==", "123", null, null, null)]
//    [DataRow("Hello,World", "FgV3tiq+guuwfn6O9xvcBQ==", "123", null, CipherMode.ECB, null)]
//    [DataRow("Hello,World", "ZSAlymayVxGI5pcdzs2GBA==", "123", "1111111111111111", null, null)]
//    public void DesDecryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new DesDecryptOption(key, _encryption.Symmetric.Des.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.Des.Decrypt(data, option);
//        var actual = Encoding.UTF8.GetString(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void DesDecryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile_DesEncrypted.txt";
//        var targetPath = "TestFile_DesDecrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.Des.DecryptFile(sourcePath, new DesDecryptOption("123", _encryption.Symmetric.Des.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }
//    #endregion

//    #region 3Des
//    [TestMethod]
//    [DataRow("9fSyKQevDL9ZsvkQhW7ypQ==", "Hello,World", "12300000000000001", null, null, null)]
//    [DataRow("wU2KxaXQ/A8bqLiGhv3opA==", "Hello,World", "123", null, CipherMode.ECB, null)]
//    [DataRow("Pm0kt7GyYp3DC9/SnMV0qw==", "Hello,World", "123", "1111111111111111", null, null)]
//    public void TripleDesEncryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new TripleDesEncryptOption(key, _encryption.Symmetric.TripleDes.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.TripleDes.Encrypt(data, option);
//        var actual = Convert.ToBase64String(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void TripleDesEncryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile.txt";
//        var targetPath = "TestFile_TripleDesEncrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.TripleDes.EncryptFile(sourcePath, new TripleDesEncryptOption("123", _encryption.Symmetric.TripleDes.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }

//    [TestMethod]
//    [DataRow("Hello,World", "9fSyKQevDL9ZsvkQhW7ypQ==", "12300000000000001", null, null, null)]
//    [DataRow("Hello,World", "wU2KxaXQ/A8bqLiGhv3opA==", "123", null, CipherMode.ECB, null)]
//    [DataRow("Hello,World", "Pm0kt7GyYp3DC9/SnMV0qw==", "123", "1111111111111111", null, null)]
//    public void TripleDesDecryptionTest(string expected, string data, string key, string iv, CipherMode? cipherMode, PaddingMode? paddingMode)
//    {
//        var option = new TripleDesDecryptOption(key, _encryption.Symmetric.TripleDes.GenerateIV(Encoding.UTF8.GetBytes(iv ?? "0000000000000000")));
//        if (cipherMode.NotNull()) option.CipherMode = cipherMode!.Value;
//        if (paddingMode.NotNull()) option.Padding = paddingMode!.Value;

//        var bytes = _encryption.Symmetric.TripleDes.Decrypt(data, option);
//        var actual = Encoding.UTF8.GetString(bytes);
//        Assert.AreEqual(expected, actual);
//    }

//    [TestMethod]
//    public void TripleDesDecryptFileTest()
//    {
//        var sourcePath = "TestData/TestFile_TripleDesEncrypted.txt";
//        var targetPath = "TestFile_TripleDesDecrypted.txt";
//        var iv = "0000000000000000";
//        var bytes = _encryption.Symmetric.TripleDes.DecryptFile(sourcePath, new TripleDesDecryptOption("123", _encryption.Symmetric.TripleDes.GenerateIV(iv.IsNull() ? null : Encoding.UTF8.GetBytes(iv!))));
//        bytes.SaveFile(targetPath);
//        Assert.IsTrue(File.Exists(targetPath));
//    }
//    #endregion

//    #region RSA KEY
//    [TestMethod]
//    public void GeneratePasswordProtectKeyPairTest()
//    {
//        //using var rsa = RSA.Create();
//        //rsa.KeySize = 2048;
//        //var bytes = rsa.ExportPkcs8PrivateKey();
//        //var privateKey = _rsaKey.FormatPkcs8Private(Convert.ToBase64String(bytes));
//        //Console.WriteLine(privateKey);
//        //var encryptedKey = _rsaKey.AddPkcs8PrivateKeyPassword(privateKey, "admin123");
//        //Console.WriteLine(encryptedKey);

//        //using var rsa = RSA.Create();
//        //rsa.KeySize = 2048;
//        //var bytes = rsa.ExportRSAPrivateKey();
//        //var privateKey = _rsaKey.FormatPkcs1Private(Convert.ToBase64String(bytes));
//        //var encryptedKey = _encryption.Symmetric.Aes.Encrypt(bytes, new AesEncryptOption("admin123",null) { CipherMode = CipherMode.CBC, Padding = PaddingMode.PKCS7 });
//        //var key = Convert.ToBase64String(encryptedKey);
//        //Console.WriteLine(_rsaKey.FormatPkcs1Private(key));
//    }

//    [TestMethod]
//    public void GenerateKeyPairTest()
//    {

//    }
//    #endregion

//    #region RSA
//    private static readonly string _rsaPublicKey = @"
//-----BEGIN RSA PUBLIC KEY-----
//MIIBCgKCAQEAs/6JeGalovo+WQCfWNbiCXfoaY50861pxKmFgrQg6STiRfEBY/z2
//RthW4TnS7Hugyo0zZam764LVXOTJUNaoBtpg6xlYXM19HR+QgwkkKXNcN/6W3Lih
//IH9mqKzkqekwu2Uu6daelU4LZx2VB+y/s+bgNS3ZvwqrPHDSADbHHN4pW7sdH9z3
//mEfkZGpieKWRcom+YBQ/6UWZCeWomxBRdUA2mY4rKhfiEdKafVeXjs9dQOaVW6Sp
//3QXMIKca/WXpsbsEyr5KCnllpCSY0CtROMSba1ozg35tLQ6egPf6Y6nxu1j5Sgv9
//830qer69cBONuWGoOzZA5Nl75ZCRE3cYuQIDAQAB
//-----END RSA PUBLIC KEY-----
//";
//    private static readonly string _rsaPrivateKey = @"
//-----BEGIN RSA PRIVATE KEY-----
//MIIEpAIBAAKCAQEAs/6JeGalovo+WQCfWNbiCXfoaY50861pxKmFgrQg6STiRfEB
//Y/z2RthW4TnS7Hugyo0zZam764LVXOTJUNaoBtpg6xlYXM19HR+QgwkkKXNcN/6W
//3LihIH9mqKzkqekwu2Uu6daelU4LZx2VB+y/s+bgNS3ZvwqrPHDSADbHHN4pW7sd
//H9z3mEfkZGpieKWRcom+YBQ/6UWZCeWomxBRdUA2mY4rKhfiEdKafVeXjs9dQOaV
//W6Sp3QXMIKca/WXpsbsEyr5KCnllpCSY0CtROMSba1ozg35tLQ6egPf6Y6nxu1j5
//Sgv9830qer69cBONuWGoOzZA5Nl75ZCRE3cYuQIDAQABAoIBAEmkASBmdqwIUAvf
//6lHRkKcpDMhrBURRFa3lYX9JLRHWClFtOhFrhGMVze3V14rzRaS5Qmb8m6rOW3bn
//D8poGFS9cdueAf3uAJsyt9A0pDvlO7yZB6YET7/Y/+pQ2jj98/cnfZskOLuDM70b
//aEbWwNPcyKxZNvYnakmPKwv1XV9cgcP1VIzz9N5+kfTYC6A2R6Z+4zllzORQh4bb
//vKXDTnEqREnS9H7TXEbysWoKAHhzNIm5g2yYBudqG8AQsSYwfiQLxNPAPpf+BJDv
//DX1XuTdcLRhRgQyetyqAvhCzb2cjOck+nyYofX4lbe+P1AEKmvS/TdlAzroqwH54
//CzTIhRECgYEA24V/zuTEbEXLj+eJMXZUhoJlfquTFO/GqQpdan7sEPVA5tU2MvVl
//7zv53UxiQyCOiyyeHS9TULYKHOHDeO6rX8ImI1YF38dFMuTZZMGplIAIvJ26t/XU
//BAuWnWK30sGniq4+uZHtOzEaJK8NhVkOHgClCGjGksC3p+7FTep3nj0CgYEA0eeK
//YjEzIGAKAK871Pop7+Hc36TWMjN3A7XU6iS8IiJUGXzqaF+L0H/LWY4SeUooI+JA
//zSBvrR6IjGK8eeuneBYUEMm7SR7ZrnwVDgjJJeDPnEVQzBr49fwIkZ3x8JhQBAm6
//dDk+1aNvKChYMRox3IhHvJ8wHzuYeERG+Uq96C0CgYEArOl9ANTFu5tGerWgJSvV
//7JWtqJcxJZ6w1tnpN0d+XuUwGCL9rCoL/uM5L41JISL15L5iVQ7AC6U4VgOm2wz0
//RFtDezX5LoliAnKsombBUs1/FEMRnnGubPlLy/DEZuhPNjeGBNnjCWrl8ttIRS8e
//pZdQZ01QjKvsfgTfuT3ktukCgYEAuRx8ycOh1vdbWucu2NAVk9uYMhLoYzVHzBnb
//PvJOh7vGdwPWpV09rA5Hgx7r49MmOx8ZDs2ALygyO7wHg23zLtApufRuGjn+twXu
//e4X7/k+YR7RJYPaYgDZYSDMGXqu0RRtfN4gUhcuwB7YssIDtYBM/UHUCt0iTWdy8
//yrTDbbECgYAJpyzpRbB/Nu4VzF5lVRARpFu4TaEmOgHr2PNtjsuRBTxjongfyUX8
//V0WHKeeNr+FyPadziqacQMRO5xehMlS+4t+jQ0nBsNOGvCrWo0RGBFdYmENY93gQ
//tWyHQKcHRhNeq1wrGYEa55m4dT4LyHHl9g5I++6DtVpJpqHZMe4wJg==
//-----END RSA PRIVATE KEY-----
//";

//    [TestMethod]
//    public void RsaEncryptTest()
//    {
//        var origin = "Hello,World";

//        var encryptOption = new RsaEncryptOption(_rsaPublicKey);
//        var encryptedBytes = _encryption.Asymmetric.Rsa.Encrypt(origin, encryptOption);
//        var encrypted = Convert.ToBase64String(encryptedBytes);
//        Console.WriteLine(encrypted);

//        var decryptOption = new RsaDecryptOption(_rsaPrivateKey) { };
//        var decryptedBytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, decryptOption);
//        var decrypted = Encoding.UTF8.GetString(decryptedBytes);

//        Assert.AreEqual(origin, decrypted);
//    }

//    [TestMethod]
//    public void RsaEncryptFileTest()
//    {
//        var originFile = @"TestData/TestFile.txt";
//        var originText = File.ReadAllText(originFile);

//        var encryptedFile = "TestFile_RsaEncrypted.txt";
//        var decryptedFile = "TestFile_RsaDecrypted.txt";

//        var encryptOption = new RsaEncryptOption(_rsaPublicKey);
//        var encryptedBytes = _encryption.Asymmetric.Rsa.EncryptFile(originFile, encryptOption);
//        encryptedBytes.SaveFile(encryptedFile);
//        Assert.IsTrue(File.Exists(encryptedFile));

//        var decryptOption = new RsaDecryptOption(_rsaPrivateKey);
//        var decryptedBytes = _encryption.Asymmetric.Rsa.DecryptFile(encryptedFile, decryptOption);
//        decryptedBytes.SaveFile(decryptedFile);
//        Assert.IsTrue(File.Exists(decryptedFile));

//        Assert.AreEqual(originText, File.ReadAllText(decryptedFile));
//    }

//    [TestMethod]
//    public void RsaSignTest()
//    {
//        var origin = "Hello,World";
//        //var expectedSign = "JvCjObnjAHeXapB3l5tNhPeihDiG6io6DjIT+ejFDVpZmAZsJV28wioSmXciYKATudzTzta5hh3KXXtt/4zQ6X/f6p36n2cEi/7agKpUyxUND6w9bkiBkT20bqPzNmmeIpp8lkyfkc7jhDLb1VMipaSUJKyuvYIcWzECdIjtnEMG3PHfgWM30UIqdOoBGwewkbZfH9Wos4I0onKo0wlGj/+eUTLdg83tP8/bekw4NqaD14HGXNHa9+XUPu9ItAJfw5HY8xQ1wmi5zdD3pO5MfY4OQtfCMp87sxkjmXzQvnR+yuVbLl8UjqGsrFAjwP88ATWlSEhKmDkDFgVrfKvZXQ==";

//        var signOption = new RsaSignOption(_rsaPrivateKey) { };
//        var signedBytes = _encryption.Asymmetric.Rsa.Sign(origin, signOption);
//        var signed = Convert.ToBase64String(signedBytes);
//        //Assert.AreEqual(expectedSign, signed);

//        var verifySignOption = new RsaVerifySignOption(signedBytes, _rsaPublicKey);
//        var verified = _encryption.Asymmetric.Rsa.VerifySign(Encoding.UTF8.GetBytes(origin), verifySignOption);
//        Assert.IsTrue(verified);
//    }

//    [TestMethod]
//    public void RsaSignFileTest()
//    {
//        var originFile = @"TestData/TestFile.txt";

//        var signOption = new RsaSignOption(_rsaPrivateKey);
//        var signedBytes = _encryption.Asymmetric.Rsa.SignFile(originFile, signOption);
//        Console.WriteLine(Convert.ToBase64String(signedBytes));

//        var verifySignOption = new RsaVerifySignOption(signedBytes, _rsaPublicKey);
//        var verified = _encryption.Asymmetric.Rsa.VerifySignFile(originFile, verifySignOption);
//        Assert.IsTrue(verified);
//    }
//    #endregion

//    [TestMethod]
//    public void Test()
//    {
//        Console.WriteLine(_encryption.Symmetric.Aes.GenerateIV().ToHexString());
//        Console.WriteLine(_encryption.Symmetric.Aes.GenerateIV().ToHexString());
//        Console.WriteLine(_encryption.Symmetric.Aes.GenerateIV().ToHexString());

//        var cert = @"
//MIIEogIBAAKCAQEAoXKv4Ww5ZWw97F6WgH+i4Utlq+Pt7mYJJJy14clRQgh/Xmis
//6zQOPoLhGrPsPl4e4r1XA0DlKvhk56MvHjRkLuwBieECZ6GBqkCokU7Z2FEpJgrv
//Q1ySCIvTOOlwmoqJX18n5NSXOET9SVk0dwmE/6HJ4U/8zvc2VCIKIiHpMjBC87NK
//63Cj/yuSffj7jhT9upyDI0fHA1ip+fKDgvXGxA+vs4Lz314Fre5twFoMeL1kCbKW
//+EuxOpLU0uc0IhZmvtKKAcdqWAYpaA0DYsyW40xee2BM0H108bGd4/vro7WKi1pR
//hP5NXhHvsmrp405WPOx8REDw8R64VK6XaBaYNwIDAQABAoIBABW5rCdGe/bE+RQ8
//qj4CbvWoQrc9xb/F5UcS+3q1r4SWRiV7KoHAEIai8A261niSC8/xMKtUaUI+CL4O
///IExBaY2xgI60CsH1b4GU2iyVqhAS6w93nKPG2ug/Tx0VV3GdLPrxx+I6Ax/ZCIh
//qei21DneGJ+ENv3W0G3D4t8y9wF0vGFjzhSNSG32DxbNGv+JkA30tDwBNz5f7Kj2
//3B/+PVb1eLQRDNyspyLamBH4uI3pyNCJ3gV4oJ68KRM1MNp57k/I/wrjd9cxlarU
//Vboq1v2JsUX+1b2aF/Okz1uTihDh+y6HeHD9NSq3a1Iqf2I/4V9nipnPGL1OXsa5
//WOk54yECgYEA0PgOLSxAoSxDkntTXBzHNEDACGDMrZqNKwWJSh+O99coazb12EDu
//AR/XHh1u2pGZ3g6fcA47H7QSXTklrEC+d2aNAfeQnRiJTuZKjgikmfOs2ZPI1rSh
//FgTWe7hpDFDGHSD6lECf2w3n1st02ZOViKDP3o3QOXc0am4trwyZzccCgYEAxciq
//gHYkV5u1mwZxqwkEpdB1oIE9yM9RWBbSzBnrqS7U/VKc0HGaYB/Zgx22132VD4lD
//q8RHuC40TXhOx8fzNo8vIGmWBbBO4HmD+TZ+OfFCBlJTSB2MnTo5IHmxEnWfZQpj
//QKofgkB44PoRKP4lFuXYkt/9DWNbbMihQXUYohECgYAU8Uc6b7zqai07ePCZwqiU
//LAkSzeC1ZU/Qh/WCYDdLz1xVomRxD9DKfA5rDjgV35G0F3MTH9n9C2brwNZttO9h
//t/1ZvR3ZIKRiK+I86dDVPP6Nr6npGqEqO8HjFFAy08WHFc8HHJLouTQK6skDJS9P
//Sg6LF1tO5wtxvykE6hZf8QKBgBhq9LnVXBlAzQPtoLzn6R37hXoiWplfFwDUGIRA
//Mvr5ICyCNX/6dn+ljvoEAh3KkEuRu7wKCHyxn51s2/UIN3ul48CRqIbdRG+sMOHk
//m2LV6TvUkBmYJFMI7CKZQ4Yu5VXZGXXTSv6OrR3BYbr43WVdTQadBqXPfWsJvfbR
//2Q+xAoGAIy7Hojf9yAZ7tCiFCStcfGALVjyxyJxXvcfXsa2LT5sPVJ+LITsIJA1o
//NpRRnSlDhthJjmLaaTECOMjGdaQUmPhf7XylIlr4wVW8KgSd9AyiTO93Lt5VRRfC
//cazRAd2RrkPIzzXaER5MY+mOCSuFJ/5fEnfnEY+hhawnRjMCwAI=
//";

//        var encryptedKey = @"
//-----BEGIN RSA PRIVATE KEY-----
//Proc-Type: 4,ENCRYPTED
//DEK-Info: AES-256-CBC,A79A07EE466BF3ED7A3794B64C279CEC

//8qoxU5uXnqALnJSE4NWTArAqSIHwuUkTTeyPls38KT8fC67NwrKmEHMDW3/7RRhW
//djDRCSausU3vLjXTeFXGy+kyCethNiYCa33NXFrABclnqJnwe6zGXq49Z7OeIEKn
//xU4TbwBCxULxbG4eGVPOU6BTlK3ATNZNklLWWGEWwm4uKaHgDOvP4+reOIMWBa7A
//3bottJ2iEgbHlFUUUoCkAJX7Uerpb5s9aM2d4aETYV8E8mGiDFAQCFL45/8HuSkG
//5pylXZc1tLDRTpnnxV+KP+KbfEXEDTtijsTd+PVci1OoYjVgIqd6Dn+118QWzTGQ
//mE9wXUw5YnLLC0Y4uonCmU9DMM7/JoQcRQe9mm8XriJJ94i8iiudgOQO8E8NBjmU
//vTYTHRbEWms2ewur6QYYOusEMTzFr+cOf3u5wam2OoUaIqG9Zwpom3PSGVPT2kJS
//wCX5Lh+2wrf+OfTlSEHmHt7pL8VcxgME4qKvGp2eZcLDsQ1t2BGi8yxcxM1nLqPy
//hZI9U2LYnfmAcBZRNekZ03qq0kT5JXrPn+uS1SpoU2gywwTKQAbeATFFj7I5LGfb
//iOl7vZ6quKA0F6y/Zc88f/l+/X+nbOXNCS7ILFiVKaO+dfZQCRvJR4iSsx6ZXpdm
//Nr5FOrCjXXlmscp4d/olS/kPU1mMhFjGcSc3Ke4QiNmaEDU5tqVu1g+Efabq6Fme
//z2uPBw4rmv5wXrJgKZkTOEq5+NXDYQgNfF/w++8p/EDHKJ3LK/XC9ARqNs382nBK
//pVkMymbixWBnnuzIT46A4zSIeOGU87hRqE6EBJ+IO6xtjNcwUnR79GovbnvgZbIA
//zujvXRL+xeD8tkWVycGDGWIK0mAbqPrYra6sNOT37yhw+THTtqLFrX9eoADA5j5v
//0Ay2rkueY3XzbzsefS+knyXKQsFrQQJbaxJCXi5w+dgTKsn4JRSdBS7xYTPj1owC
//6bJS0cHDgCILghxzDboaePTla2jBYfVt1a5Z1N2Jq4QthOQapkOMkMMiB3hEXc1H
//TWWgPFc6xn2FCprrkN727djBr2/i4+r/2WUkXuohffH+KXP0nXeWoLhcmyG3+Sdv
//R8FZfQqTl2vPmpRBnwKkfU/ZTQ1/o5Wuf7exP6xFxosB5bz6rOJdKvaTAWCtOCum
//Bm0q3L5hcg8Ad73sE3ZU3s0Zx67Q7tGXM16/w5jxfrAAXbvCWG+AFPbuABwGZXYD
//paA0P2Np5Oz/OISSLG63sNUWes4nG22creLa1eNnsJXAmvokwWNgz6lMyxovwnP0
//tJT5xW9iVk4wy06DiANdcmdbNtQpgMzfcfjJIiw3UDcSi3KqyzRwjf3IxIqItTM9
//FSb+MkEgn4gOHJDvDax6Q0wwjlIOvEjkv/pdB25oi7n3957IwrLyq+IerKMBC6BY
//QnceCZPpnNiPUX6amKXjdh65eTNqnKomqHfTEyvlJMK0ON6YWJwSpYVciepEQW3O
//lvpnTaA4Mgma4nd/LO2CbuF3ZqD+dWMNUlrX3skLvKUBXUU6KGcn8mc/8J13YSyR
//B3WrKKSZG6ihKiPFRwftNQU6kY1KRNDq+KRRt4oOBwfIn2PWL22ZeYvnp71BpF0E
//-----END RSA PRIVATE KEY-----
//";

//        var key = "8qoxU5uXnqALnJSE4NWTArAqSIHwuUkTTeyPls38KT8fC67NwrKmEHMDW3/7RRhWdjDRCSausU3vLjXTeFXGy+kyCethNiYCa33NXFrABclnqJnwe6zGXq49Z7OeIEKnxU4TbwBCxULxbG4eGVPOU6BTlK3ATNZNklLWWGEWwm4uKaHgDOvP4+reOIMWBa7A3bottJ2iEgbHlFUUUoCkAJX7Uerpb5s9aM2d4aETYV8E8mGiDFAQCFL45/8HuSkG5pylXZc1tLDRTpnnxV+KP+KbfEXEDTtijsTd+PVci1OoYjVgIqd6Dn+118QWzTGQmE9wXUw5YnLLC0Y4uonCmU9DMM7/JoQcRQe9mm8XriJJ94i8iiudgOQO8E8NBjmUvTYTHRbEWms2ewur6QYYOusEMTzFr+cOf3u5wam2OoUaIqG9Zwpom3PSGVPT2kJSwCX5Lh+2wrf+OfTlSEHmHt7pL8VcxgME4qKvGp2eZcLDsQ1t2BGi8yxcxM1nLqPyhZI9U2LYnfmAcBZRNekZ03qq0kT5JXrPn+uS1SpoU2gywwTKQAbeATFFj7I5LGfbiOl7vZ6quKA0F6y/Zc88f/l+/X+nbOXNCS7ILFiVKaO+dfZQCRvJR4iSsx6ZXpdmNr5FOrCjXXlmscp4d/olS/kPU1mMhFjGcSc3Ke4QiNmaEDU5tqVu1g+Efabq6Fmez2uPBw4rmv5wXrJgKZkTOEq5+NXDYQgNfF/w++8p/EDHKJ3LK/XC9ARqNs382nBKpVkMymbixWBnnuzIT46A4zSIeOGU87hRqE6EBJ+IO6xtjNcwUnR79GovbnvgZbIAzujvXRL+xeD8tkWVycGDGWIK0mAbqPrYra6sNOT37yhw+THTtqLFrX9eoADA5j5v0Ay2rkueY3XzbzsefS+knyXKQsFrQQJbaxJCXi5w+dgTKsn4JRSdBS7xYTPj1owC6bJS0cHDgCILghxzDboaePTla2jBYfVt1a5Z1N2Jq4QthOQapkOMkMMiB3hEXc1HTWWgPFc6xn2FCprrkN727djBr2/i4+r/2WUkXuohffH+KXP0nXeWoLhcmyG3+SdvR8FZfQqTl2vPmpRBnwKkfU/ZTQ1/o5Wuf7exP6xFxosB5bz6rOJdKvaTAWCtOCumBm0q3L5hcg8Ad73sE3ZU3s0Zx67Q7tGXM16/w5jxfrAAXbvCWG+AFPbuABwGZXYDpaA0P2Np5Oz/OISSLG63sNUWes4nG22creLa1eNnsJXAmvokwWNgz6lMyxovwnP0tJT5xW9iVk4wy06DiANdcmdbNtQpgMzfcfjJIiw3UDcSi3KqyzRwjf3IxIqItTM9FSb+MkEgn4gOHJDvDax6Q0wwjlIOvEjkv/pdB25oi7n3957IwrLyq+IerKMBC6BYQnceCZPpnNiPUX6amKXjdh65eTNqnKomqHfTEyvlJMK0ON6YWJwSpYVciepEQW3OlvpnTaA4Mgma4nd/LO2CbuF3ZqD+dWMNUlrX3skLvKUBXUU6KGcn8mc/8J13YSyRB3WrKKSZG6ihKiPFRwftNQU6kY1KRNDq+KRRt4oOBwfIn2PWL22ZeYvnp71BpF0E";
//        var decrypted = _encryption.Symmetric.Aes.Decrypt(Convert.FromBase64String(key), new AesDecryptOption("admin123", "A79A07EE466BF3ED7A3794B64C279CEC".FromHexString()) { Padding = PaddingMode.PKCS7, CipherMode = CipherMode.CBC });
//        ////var bytes = Convert.FromBase64String(encryptedKeye.Trim());
//        //var data = new AsnEncodedData(decrypted);
//        //var decode = data.Format(true);
//        //var decodeStr = Convert.ToBase64String(data.RawData);
//        //Console.WriteLine(decode);
//        //Console.WriteLine(decodeStr);
//        //Console.WriteLine(_rsaKey.FormatPkcs1Private(decodeStr));
//        //AsnDecoder.ReadSequence(Convert.FromBase64String(cert.Trim()), AsnEncodingRules.DER, out var _, out var _, out var _);
//        //Console.WriteLine(decoded);
//        //Console.WriteLine(_rsaKey.FormatPkcs1Private(Convert.ToBase64String(decoded)));

//        //var bytes = Encoding.UTF8.GetBytes(encryptedKey);

//        using var txtReader = new StringReader(encryptedKey.Trim());
//        var reader = new PemReader(txtReader, new Password("admin123".ToCharArray()));
//        var obj = reader.ReadObject();
//        var pem = reader.ReadPemObject();

//        var asn = new AsnReader(decrypted, AsnEncodingRules.BER);
//        var tag = asn.PeekTag();
//    }

//    private class Password
//            : IPasswordFinder
//    {
//        private readonly char[] password;

//        public Password(
//            char[] word)
//        {
//            this.password = (char[])word.Clone();
//        }

//        public char[] GetPassword()
//        {
//            return (char[])password.Clone();
//        }
//    }
//}