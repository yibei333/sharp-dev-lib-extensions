using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Encryption;
using System;
using System.IO;
using System.Text;

namespace SharpDevLib.Extensions.Tests.Encryption;

[TestClass]
public class RsaTests : EncryptionTestBase
{
    #region xml
    private readonly string _xmlPrivate = @"
<RSAKeyValue>
      <DP>EioULUHcHzQLyvFzG4oMb7LpxMyMisiP9CGATTl7bWfITNhhndYvb5ZVCwGjTiKdjKJklsuRf2c5BREbtjZgAQ==</DP>
      <DQ>5pY+/BEY8p9ZaOGG5P9xdWIZz2Z2SUzA8gfbyRxZu2iSyLvrlGpaS032xF/VV8M/bKG8mY+qQPoWSWnNdBIj0Q==</DQ>
      <D>EBPFnGhZ6mJxYit0UEU4yEaE6xziw4rRNpDIEHCvdHS/DpDiqAfbEMy2QjH8NnvDwuxj9wG0UTPq+xnx1nMsHGWP/skZBoSXiz6/fQgL66JZEKZ04PNOJ5PwMCq8488ifzKoYP63WM7tjzXURlXQp0eMkRBMlsy0IkjVzsiKAAE=</D>
      <Modulus>oJGpFlBM4iFiubtvVLeOTEf8UYdKX+BZ8FEXsHGZrP1ACBzCyT8WQz+1rXWFhOAs5CaoMsgHmpOhZTwJsMQZRQK7NHOCToDaxbX2dIyQvKxZ57G+hKRqJxAdv2EF/YW6bhk99WpweJ/Q3GvckKCgmhywYYDwjpbJNVR//7cNfGU=</Modulus>
      <Exponent>AQAB</Exponent>
      <InverseQ>PYrZVkHi+VM99IQ5P4549cgA2XcQ/d+SPosHsD+X7t2wvm7Yf9kRFki3D7Al769Xjz2KfFqtK+4sARHGXie3pQ==</InverseQ>
      <P>qUPAo9+U4Kp102tFqzsA0/gLozEm9b0yXb8Bci2rSyMK1WaOHOTtrr9EhE1AdLHpL7fTx/MyquMT2TPTRrpgAQ==</P>
      <Q>8tk0QY873Uel22ov6aV83lzns4Pys/AtzrPI+fYa/Hi2ASWFJM1+Dow+RxUen0rztQhyYU2ammeJI5gHXQWcZQ==</Q>
</RSAKeyValue>
";
    private readonly string _xmlPublic = @"
<RSAKeyValue>
  <Modulus>oJGpFlBM4iFiubtvVLeOTEf8UYdKX+BZ8FEXsHGZrP1ACBzCyT8WQz+1rXWFhOAs5CaoMsgHmpOhZTwJsMQZRQK7NHOCToDaxbX2dIyQvKxZ57G+hKRqJxAdv2EF/YW6bhk99WpweJ/Q3GvckKCgmhywYYDwjpbJNVR//7cNfGU=</Modulus>
  <Exponent>AQAB</Exponent>
</RSAKeyValue>
";

    [TestMethod]
    public void XmlDecryptStringTest()
    {
        var encrypted = "ZXoKnp4qumNCKlHCDMLZWzws6uP0f7/SbRTirjxRpfIHLov2ORQ4BB2sD2GmTN9oBOJpP4oQAcYuBuV7TJPv88C9fwMaQrgh62vqdk2DUHDMKBLtoy4CEAcWiW2NVDVi8HlfnMr2Vu36W7l7VPv8lCq7SuJcIeYD9kdQPSA+xRE=";
        var expected = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, new RsaDecryptOption(_xmlPrivate));
        var actual = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void XmlEncryptStringTest()
    {
        var expected = "ZXoKnp4qumNCKlHCDMLZWzws6uP0f7/SbRTirjxRpfIHLov2ORQ4BB2sD2GmTN9oBOJpP4oQAcYuBuV7TJPv88C9fwMaQrgh62vqdk2DUHDMKBLtoy4CEAcWiW2NVDVi8HlfnMr2Vu36W7l7VPv8lCq7SuJcIeYD9kdQPSA+xRE=";
        var plainText = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Encrypt(plainText, new RsaEncryptOption(_xmlPublic));
        var actual = Convert.ToBase64String(bytes);
        Console.WriteLine(actual);
        //every result is different,cause secure random
        Assert.AreEqual(expected.Length, actual.Length);
    }

    [TestMethod]
    public void XmlSignStringTest()
    {
        var plainText = "Hello,World";
        var expected = "O3MT+RUsc3pq2+VguTTlQSGuQS1p9amUv50UkJSqEBCEGQiZ0t8yYhA0Y4Q2Vio+oZGQ2saVQb+Q3jPsYW4AuNIgxsdC1lVnum5ZomGkUyNhyBlg3qHIqGVKfRqLcREhOJpUxAPT1UW82EM5zFQYFeQyrhSEhqgaqm81D17Z2Q0=";
        var bytes = _encryption.Asymmetric.Rsa.Sign(plainText, new RsaSignOption(_xmlPrivate));
        var actual = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void XmlVerifySignStringTest()
    {
        var plainText = "Hello,World";
        var signature = "O3MT+RUsc3pq2+VguTTlQSGuQS1p9amUv50UkJSqEBCEGQiZ0t8yYhA0Y4Q2Vio+oZGQ2saVQb+Q3jPsYW4AuNIgxsdC1lVnum5ZomGkUyNhyBlg3qHIqGVKfRqLcREhOJpUxAPT1UW82EM5zFQYFeQyrhSEhqgaqm81D17Z2Q0=";
        var isMatch = _encryption.Asymmetric.Rsa.VerifySign(plainText, new RsaVerifySignOption(Convert.FromBase64String(signature), _xmlPublic));
        Assert.IsTrue(isMatch);
    }
    #endregion

    #region pkcs1
    private readonly string _pkcs1Private = @"
-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQCgkakWUEziIWK5u29Ut45MR/xRh0pf4FnwURewcZms/UAIHMLJ
PxZDP7WtdYWE4CzkJqgyyAeak6FlPAmwxBlFArs0c4JOgNrFtfZ0jJC8rFnnsb6E
pGonEB2/YQX9hbpuGT31anB4n9Dca9yQoKCaHLBhgPCOlsk1VH//tw18ZQIDAQAB
AoGAEBPFnGhZ6mJxYit0UEU4yEaE6xziw4rRNpDIEHCvdHS/DpDiqAfbEMy2QjH8
NnvDwuxj9wG0UTPq+xnx1nMsHGWP/skZBoSXiz6/fQgL66JZEKZ04PNOJ5PwMCq8
488ifzKoYP63WM7tjzXURlXQp0eMkRBMlsy0IkjVzsiKAAECQQCpQ8Cj35TgqnXT
a0WrOwDT+AujMSb1vTJdvwFyLatLIwrVZo4c5O2uv0SETUB0sekvt9PH8zKq4xPZ
M9NGumABAkEA8tk0QY873Uel22ov6aV83lzns4Pys/AtzrPI+fYa/Hi2ASWFJM1+
Dow+RxUen0rztQhyYU2ammeJI5gHXQWcZQJAEioULUHcHzQLyvFzG4oMb7LpxMyM
isiP9CGATTl7bWfITNhhndYvb5ZVCwGjTiKdjKJklsuRf2c5BREbtjZgAQJBAOaW
PvwRGPKfWWjhhuT/cXViGc9mdklMwPIH28kcWbtoksi765RqWktN9sRf1VfDP2yh
vJmPqkD6FklpzXQSI9ECQD2K2VZB4vlTPfSEOT+OePXIANl3EP3fkj6LB7A/l+7d
sL5u2H/ZERZItw+wJe+vV489inxarSvuLAERxl4nt6U=
-----END RSA PRIVATE KEY-----
";
    private readonly string _pkcs1Public = @"
-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCgkakWUEziIWK5u29Ut45MR/xR
h0pf4FnwURewcZms/UAIHMLJPxZDP7WtdYWE4CzkJqgyyAeak6FlPAmwxBlFArs0
c4JOgNrFtfZ0jJC8rFnnsb6EpGonEB2/YQX9hbpuGT31anB4n9Dca9yQoKCaHLBh
gPCOlsk1VH//tw18ZQIDAQAB
-----END PUBLIC KEY-----
";

    [TestMethod]
    public void Pkcs1DecryptStringTest()
    {
        var encrypted = "ZXoKnp4qumNCKlHCDMLZWzws6uP0f7/SbRTirjxRpfIHLov2ORQ4BB2sD2GmTN9oBOJpP4oQAcYuBuV7TJPv88C9fwMaQrgh62vqdk2DUHDMKBLtoy4CEAcWiW2NVDVi8HlfnMr2Vu36W7l7VPv8lCq7SuJcIeYD9kdQPSA+xRE=";
        var expected = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, new RsaDecryptOption(_pkcs1Private));
        var actual = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Pkcs1EncryptStringTest()
    {
        var expected = "ZXoKnp4qumNCKlHCDMLZWzws6uP0f7/SbRTirjxRpfIHLov2ORQ4BB2sD2GmTN9oBOJpP4oQAcYuBuV7TJPv88C9fwMaQrgh62vqdk2DUHDMKBLtoy4CEAcWiW2NVDVi8HlfnMr2Vu36W7l7VPv8lCq7SuJcIeYD9kdQPSA+xRE=";
        var plainText = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Encrypt(plainText, new RsaEncryptOption(_pkcs1Public));
        var actual = Convert.ToBase64String(bytes);
        Console.WriteLine(actual);
        //every result is different,cause secure random
        Assert.AreEqual(expected.Length, actual.Length);
    }

    [TestMethod]
    public void Pkcs1SignStringTest()
    {
        var plainText = "Hello,World";
        var expected = "O3MT+RUsc3pq2+VguTTlQSGuQS1p9amUv50UkJSqEBCEGQiZ0t8yYhA0Y4Q2Vio+oZGQ2saVQb+Q3jPsYW4AuNIgxsdC1lVnum5ZomGkUyNhyBlg3qHIqGVKfRqLcREhOJpUxAPT1UW82EM5zFQYFeQyrhSEhqgaqm81D17Z2Q0=";
        var bytes = _encryption.Asymmetric.Rsa.Sign(plainText, new RsaSignOption(_pkcs1Private));
        var actual = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Pkcs1VerifySignStringTest()
    {
        var plainText = "Hello,World";
        var signature = "O3MT+RUsc3pq2+VguTTlQSGuQS1p9amUv50UkJSqEBCEGQiZ0t8yYhA0Y4Q2Vio+oZGQ2saVQb+Q3jPsYW4AuNIgxsdC1lVnum5ZomGkUyNhyBlg3qHIqGVKfRqLcREhOJpUxAPT1UW82EM5zFQYFeQyrhSEhqgaqm81D17Z2Q0=";
        var isMatch = _encryption.Asymmetric.Rsa.VerifySign(plainText, new RsaVerifySignOption(Convert.FromBase64String(signature), _pkcs1Public));
        Assert.IsTrue(isMatch);
    }
    #endregion

    #region encrypted pkcs1
    private readonly string _encryptedPkcs1Private = @"
-----BEGIN RSA PRIVATE KEY-----
Proc-Type: 4,ENCRYPTED
DEK-Info: DES-EDE3-CBC,0A94A99DAA11FF9F

6xfMClCQnEx0Ce7bOUFZlpxgfgBw8TWrzg1lVHA76qas7dFcFjKW6q3pctCaqGoz
zUNbshPZeYre3kGrAZ2KbPFPT7yAO5a0IO4WneSb8sNE3iYUHRpCbQK4CcvHLOmG
+XcupnzJ6Sr8+R3Tl59zrHSh7L+jt0keXEJJKoppqr3ZqIYR5+V9uldOSQfOzE8M
OZq9KeQ8/oKtSrl1+N/Ef+BOZENzYJd1L1jCwTViMFHTwYoKmBDiX9T9n4LRwd3N
JGzi9ptW9iWjVSe8H1QmRLPCcBXv006A8bkaNypQNB/m1wbxup3i4LqE4we6M8Rs
DOHmVWkBAFZOUZdraJO2iY1pCr5jHEzG4N3MkFgJrQdBAAIv4ajQeKAtoVhHrqKr
uOQEmjOILicWVCfhpfftfOKbgG42/GbtMgKW3l4/7aHdcECj1am53kzru4C5LebH
fkAsQVV1ZqX5ge0CwaRHsHxzpthw3zasTryGvGgqJpf9HU8oDxMnt46xhx68DyDm
XCCw8bYsEkpDZ26XKtm1MYimaOXVSXIvV5sS0LKMu41cmoWfCnI35XhoPK0pm2Df
fVhtjyQWm3OuF1k4Oy8CAHx6IxNvAiqDLu/iqYmaqH/QjdWL6DGFQX1J9qVupGcP
gLyytoqlj2360r2hQ75tRz4vVOPx0tBJ0ffSgqUpFwIiW4Iz9bXkZjy2CLt/PJLW
EI1Xpu9/FNRVFQB1EtsTzxLFZ9nJ32G02LrB1JZJT85JkuF0gIGBMrjjMRS5mN9A
Hf7bL5h8RGEVV+t0XxFItzc1+l/XS5kBXCPh3Y3334r4Aih/g9e8TQ==
-----END RSA PRIVATE KEY-----
";
    private readonly string _encryptedPkcs1Public = @"
-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGBI3PDvSzNLWmIbLvOSVgEGux
w5HVihR7FZxR+ni5OvYnKGIcI6UKVx8uBbugnWU6p0hdpVr96dsKqDX93Z1k7IJG
TLgf6ZF3AluWkmGt3+ZFgXkwTjOHZAWqn5kU6p2jqUMUcaMLzQBImFgQS3EJayw1
PAoxqBxoh2PHT0avjwIDAQAB
-----END PUBLIC KEY-----
";
    private readonly string _pkcs1Password = "123456";

    [TestMethod]
    public void EncryptedPkcs1DecryptStringTest()
    {
        var encrypted = "JSdD44mChUGzDjIunBzllFgtprOlQN/UpymS8Fjou4qP1AO5Ao52E7ZHRaJRHmW4FubRQ8j3huylik2YnEWtI0GBqDPZs6LVAlaMlu8sA9yTdduDAriOocmwFi7opPoSGZ7IIH3KGd3m+ZvqGK+1wMyT3VZnrYcUFcpiurqR29Q=";
        var expected = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, new RsaDecryptOption(_encryptedPkcs1Private) { PrivateKeyPassword = _pkcs1Password });
        var actual = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void EncryptedPkcs1EncryptStringTest()
    {
        var expected = "JSdD44mChUGzDjIunBzllFgtprOlQN/UpymS8Fjou4qP1AO5Ao52E7ZHRaJRHmW4FubRQ8j3huylik2YnEWtI0GBqDPZs6LVAlaMlu8sA9yTdduDAriOocmwFi7opPoSGZ7IIH3KGd3m+ZvqGK+1wMyT3VZnrYcUFcpiurqR29Q=";
        var plainText = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Encrypt(plainText, new RsaEncryptOption(_encryptedPkcs1Public));
        var actual = Convert.ToBase64String(bytes);
        Console.WriteLine(actual);
        //every result is different,cause secure random
        Assert.AreEqual(expected.Length, actual.Length);
    }

    [TestMethod]
    public void EncryptedPkcs1SignStringTest()
    {
        var plainText = "Hello,World";
        var expected = "QRwd6jxQ2Z+o/lp1SMeRYYOpyC5IabwM524y4GpiSGxaAeSTAImi9+88MfZd7Gk8OVOY8GTZ1Zd+f+Dj25/0uYx+h493WaHNNaLcolrPf9M/DTkSbgwKftih9l6Cc1sfxQnxbCDMoxCJMrFDOgFmF9xlV/7Tm/oBoIqwr5knoEU=";
        var bytes = _encryption.Asymmetric.Rsa.Sign(plainText, new RsaSignOption(_encryptedPkcs1Private) { PrivateKeyPassword = _pkcs1Password });
        var actual = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void EncryptedPkcs1VerifySignStringTest()
    {
        var plainText = "Hello,World";
        var signature = "QRwd6jxQ2Z+o/lp1SMeRYYOpyC5IabwM524y4GpiSGxaAeSTAImi9+88MfZd7Gk8OVOY8GTZ1Zd+f+Dj25/0uYx+h493WaHNNaLcolrPf9M/DTkSbgwKftih9l6Cc1sfxQnxbCDMoxCJMrFDOgFmF9xlV/7Tm/oBoIqwr5knoEU=";
        var isMatch = _encryption.Asymmetric.Rsa.VerifySign(plainText, new RsaVerifySignOption(Convert.FromBase64String(signature), _encryptedPkcs1Public));
        Assert.IsTrue(isMatch);
    }
    #endregion

    #region pkcs8
    private readonly string _pkcs8Private = @"
-----BEGIN PRIVATE KEY-----
MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCkMlj1ZcpbKc+z
Wyma54uzQWioTgIDSrtIcGjybv8PoHYRnR1vAJ1MB7iVVNplVqg1cH6FoLhhXvfd
vnDiAD1r7SmoxXYF6ZGpqvcFHGpG5o4qk1H2bPNGPTSXf2WSatg3b3EKSnYyyRMh
+WWykDkIKxwIK1GM5l5UFov1pO8mfGSM4+F8KVA9QwQAVQ01FXqsxQHVQuUX4ZQU
l+f3P+kmFsB0gI1vs89fYMKeXFZe6sKK607CYbMjxIwxe7AAyYzkG/NceRAtWrv/
99QR2eG6i3v/VTpeq5QQbeGzFBITIetNuKsiNDJXIB6+ODjsms/uoGNwnOKuO9XW
CfUKdCYVAgMBAAECggEAA1PB2bcmUpKWUtwWC72FZyx8ImzyPX7WLLGabs8w+/AU
dGbqocUxasj9RIljo4oN/oc5CRwooFB8VVH2l53JoNY9cd4OdjA1c2qzdrcMsEMb
vmEd2BB9HX9AAZHjTQQ9PWJUuBCqNH0+lgGnveaOQUweVUwLav+ACdQrId8luXHj
h5q2CJShFsnQUdgdx/sEzA5KcO5TF4bRD08cHMeiomhSdzVmoAczpntJ+g/pPoO7
94CqhJ44BCAh8s8tygZevnaAJ+uQPsYkSuuetGHe6Ds04KAh9cdfiPhkNIjnsgQB
mlytsy0UX/g3+uQcGP6NcIxeiifvef1mWFyMlMyeQQKBgQDTaFdh6x4BgPVAyhLH
SpZEm7GDOraKFt2mXNrGnrqoS1o9m2j1sZkjg9dsQI0C/Mxb5bihk8HN6zxWPDg+
U2QPiDHtPDfR3qKFMELJil0uvPsljdat9KbMP9rw9PdQiWgZicVJhJA9bLbGrCYS
nXf0FI63OUe/5f4FYQsWg9dJhwKBgQDG1LH3OKCTeW5R7bFhqAeLEHscDhA03S10
P2q8WuHleQKC8EAbUpJAWjLjNuRHWXK97MlrZHVdDt1mvYcvIcyqsUO8aAURIk7B
ZttINy705gYZj1806cL8LqkTQ0u/bsuguwdq5ZYp6xFzOuvKVlYNX+OVXi5EVrkf
igcrX9PKgwKBgCKdpClBYWFnp0DblaWSPfav86oENB16TwLNk3kQRooGBJmv18jb
MrMRBHYSP8Ssdh3aCJhUZFh+WxvXsnHqH6k3aM1qnc08gioC0/zCLi3X/duRVQSY
2bL6BbSMt+41AZmk6Gv4S9Hy+h6f04AazqhlsVSpCWmPlD93O2SNY/QDAoGAbpQb
5cI4i8wc7rOi0w0AJPclgJTRrpTTkrMySU7CEz0ZiHjVLlzduaxwYK1Gh+fXSIlH
+9IC854ww9LkO/IwQT4UzdOL/dt2sVji22bVVp4XBmAWMvdO2bGZlnz2w6B63wkl
Nu44dVemfM9oEz+w2/9YMtoOITOaAVBibFz/DMECgYBejXFn8VJ/7bVQMwtMA3xM
pePn3pS+QBtMzKrMTlyVZ9m9DY2X/2H2ZZJiMjpJ/QKeCGrR6Jg1bjZM7P2jIyFW
GAwXOHW2gnrRDlVlhE5YrJzNbJeIgybs0bERsN+ryhWpjsOL9hrcaiJ+/BM0XijE
x7j011EtmPBjA9ojd0Vjmg==
-----END PRIVATE KEY-----
";
    private readonly string _pkcs8Public = @"
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApDJY9WXKWynPs1spmueL
s0FoqE4CA0q7SHBo8m7/D6B2EZ0dbwCdTAe4lVTaZVaoNXB+haC4YV733b5w4gA9
a+0pqMV2BemRqar3BRxqRuaOKpNR9mzzRj00l39lkmrYN29xCkp2MskTIfllspA5
CCscCCtRjOZeVBaL9aTvJnxkjOPhfClQPUMEAFUNNRV6rMUB1ULlF+GUFJfn9z/p
JhbAdICNb7PPX2DCnlxWXurCiutOwmGzI8SMMXuwAMmM5BvzXHkQLVq7//fUEdnh
uot7/1U6XquUEG3hsxQSEyHrTbirIjQyVyAevjg47JrP7qBjcJzirjvV1gn1CnQm
FQIDAQAB
-----END PUBLIC KEY-----
";

    [TestMethod]
    public void Pkcs8DecryptStringTest()
    {
        var encrypted = "pAbI8jlrBvkYsa/qyaG9muDcM3PhPPx74jsq+Nh0eMLwXA4FphMYlyVuZAKiMKPD7RDDNw+B7Auqe8Aae6EG4wv6Riol8U5Z1wC7UCEHZkLLv0ee58GuJwsZoCeuxI/4ZML9/TQwkL9UEtS/jJBZhRGZlOOjAktzxig3RuAeZFRuU9hIPxSEWpmids57IgngBvAm3v32aTcZegt91zwWx1C/LZI9r8gpaqA+J3mAIA3md5WElFYUADolpqgbeKdRcari9MymM/NADt5JZ3RBxrYbTzhWJ/s+6r+G87NEss3BPDb7p2U8ObmhxGkgMTMY6YUXyUvvPCwxCE9xzlpcIA==";
        var expected = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, new RsaDecryptOption(_pkcs8Private));
        var actual = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Pkcs8EncryptStringTest()
    {
        var expected = "pAbI8jlrBvkYsa/qyaG9muDcM3PhPPx74jsq+Nh0eMLwXA4FphMYlyVuZAKiMKPD7RDDNw+B7Auqe8Aae6EG4wv6Riol8U5Z1wC7UCEHZkLLv0ee58GuJwsZoCeuxI/4ZML9/TQwkL9UEtS/jJBZhRGZlOOjAktzxig3RuAeZFRuU9hIPxSEWpmids57IgngBvAm3v32aTcZegt91zwWx1C/LZI9r8gpaqA+J3mAIA3md5WElFYUADolpqgbeKdRcari9MymM/NADt5JZ3RBxrYbTzhWJ/s+6r+G87NEss3BPDb7p2U8ObmhxGkgMTMY6YUXyUvvPCwxCE9xzlpcIA==";
        var plainText = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Encrypt(plainText, new RsaEncryptOption(_pkcs8Public));
        var actual = Convert.ToBase64String(bytes);
        Console.WriteLine(actual);
        //every result is different,cause secure random
        Assert.AreEqual(expected.Length, actual.Length);
    }

    [TestMethod]
    public void Pkcs8SignStringTest()
    {
        var plainText = "Hello,World";
        var expected = "YYUjPFDYped1/ZGLyRrRVz1NMvBXq3fg3go/CTH3uFRYX/iGrzGMVzO/BbuNKOd/TRGgQRmoocQcByuoxFilGlqmCghzdV/NGj/89NY9ZVVT383fhQn0CguRPf4zVpPoEJkv3cRd9HPOeigKq17TKb+xZCwMrLZq7cl5J/upMAkfDRD4K1I3XwqIQZllEjRqjK/Nbqrg6nVv69WSAlKx1YL083sgJnMMRCzZuGkxCTXKPeGbkL7tV5OLnxVQIND1I1v7WsNNEMdchFP01y5FH9aVPuKYWT+MR6GB+cZBStiHa1Wc58byRS0NEm0RqW6vh3jtaBBpfeUCIfAbAEku5A==";
        var bytes = _encryption.Asymmetric.Rsa.Sign(plainText, new RsaSignOption(_pkcs8Private));
        var actual = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Pkcs8VerifySignStringTest()
    {
        var plainText = "Hello,World";
        var signature = "YYUjPFDYped1/ZGLyRrRVz1NMvBXq3fg3go/CTH3uFRYX/iGrzGMVzO/BbuNKOd/TRGgQRmoocQcByuoxFilGlqmCghzdV/NGj/89NY9ZVVT383fhQn0CguRPf4zVpPoEJkv3cRd9HPOeigKq17TKb+xZCwMrLZq7cl5J/upMAkfDRD4K1I3XwqIQZllEjRqjK/Nbqrg6nVv69WSAlKx1YL083sgJnMMRCzZuGkxCTXKPeGbkL7tV5OLnxVQIND1I1v7WsNNEMdchFP01y5FH9aVPuKYWT+MR6GB+cZBStiHa1Wc58byRS0NEm0RqW6vh3jtaBBpfeUCIfAbAEku5A==";
        var isMatch = _encryption.Asymmetric.Rsa.VerifySign(plainText, new RsaVerifySignOption(Convert.FromBase64String(signature), _pkcs8Public));
        Assert.IsTrue(isMatch);
    }
    #endregion

    #region encrypted pkcs8
    private readonly string _encryptedPkcs8Private = @"
-----BEGIN ENCRYPTED PRIVATE KEY-----
MIIFDjBABgkqhkiG9w0BBQ0wMzAbBgkqhkiG9w0BBQwwDgQIEbtAWHwl1+sCAggA
MBQGCCqGSIb3DQMHBAjjzVF1p3FL5QSCBMgWGhcrBKyyYZ3mekQ/Jcf0EtGLCvx+
iyU3RDPyH1raajWTSLjc4UpvwXFmt+vYjFab+ryqA+78yg0naIg1TMj7DfI0w7H5
6NTxrK+PUS62C2L598xuE6axzn9tLRsaqZJGCTO0I1mOgjyPhk9TBV5jb2I2MlA7
fkvYKvbnLOhzKtO4Ao6aTFv58Lu0LtIJpqHZh9m6gtrqhk08w9iI44HsbnMz1ziA
eHuthsXswMVVkegL39dg6zuhiXY28XMNInHfbI2hh64eMmsmyPvEQ29r0juKC1M7
ZpQW3u8hoXLwPMTx2yZn/Cx56dCCZ59zba9DxsFG2E17N3wn/b1X7kSOeqi4jbgC
oXaXqRtYBLAEg2NeY/ervPAJZaqaYMf01PM4SXegAKHqJl6YR8BVoS9nozykgKHF
dAXbOOU+6ZY2ufTrr5QnAjzYkZNdbmsSGSJODgcu/tt9geaWQa83CHVnT8SIwXv7
fBL64RhLTZ4v2HjjfQDPSkPBFvnQ80rOLG/Q+weWLp5A8CpStS4qNWmhMyHnU+iR
2T+lzDqagTTRwPHioXn3/gvGVSoKKELY04HOf24LO6VdWitT2Jg4DN9xLtuNgxXV
cZBQMLkYrd9/zV0NNHAz39tFa0JXw6MWMiWMLlTclyWamMqn5Z60mVyehmMpcQvR
YHisOL5AVXO/PPPZYdZ56mgJknLvdmDHb4mK08J+THlShscW/NoaaO3pq4wbmk+n
twczKaxENOdmuYHri/1Na0DqQQM//icSUwtTNHlMf633niGUjBVmsGmDLHLvk+Lu
KaJ3YyA+i+eGZFy1etOCrd52G3Q36UNqSxeoHffVlj/4vpHqaFZxG/t2CC8XZnFo
RvqDbslV5BbREATfY4pWUGhcE4SiNK49/8aH11/uzLKqXTHjfyoWx2NFy/SjGYWQ
IwBJsVpj0rIjoV8WP0dmXGTXQ1WKpTYHPJbn9gqTkVnn3ipwgNIMrJBAl/9oKw2J
sqv3EHJA7RifGQ9MUnSe4+5ATShPsxOt/o0Ce8rOm3dKDWiFLR2MWoMsFOlUeaRI
UI60MFgE/5o4+XSnal2C5Lquy6ouYAfvcitXc0caFP+12uu8Nv/ANYkiHcXKqYhp
RD6RPkflBrg6sDn9VkLyG+4twPUbLyYeRce68l21tO9qIvUT1agcvdKuOvhIpCU2
P5RE43FYB4IY9Zi01NMc+AndWoWz6IVxWUFrbphu2yzuW3Q2iuOxdBc9CpZRUMaF
EPsnP7mnslnQaHLCdl9u4WkIGxE6S9c8rjJaoDAyY/ZthtHDBltK9vxTTVd78SvE
CR3uwB9KSgfKtaYxtzkQemPHBOmZWJEOFD0sj/D0vtwGFtELy7VcXErDwd2+tUZ+
3KaUDvxiBj9bT6mcXh+3BVospUvddGA2zgecBLhz2shUv+eDPIwA5OsRHPZRa7Wx
irPS7iISJ63Tsa1hsxpondPw70OauBsuOnrgxwYmegTQ5JIaY9gvrMw1Fl0Eb+bv
Ts1SsPnxS/P1UXUillBPOIncyvwdk7p01UNuQzQCc92INBLJX7Me0VRQqPeX3MfF
K0g9bFRmG6r6H/nIDW4+aRz7s0AO5klRvTV+81B5c+0XHdEwZSf5nIuDKmKWV7Rr
MS4=
-----END ENCRYPTED PRIVATE KEY-----
";
    private readonly string _encryptedPkcs8Public = @"
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA/NH0i6bA0e/3aFhV4iXT
JjB0R8+9mN1uN3Clsqn3umyXY3FcXD0iidubL7CY4pXgNXXGpJUBJeC1qrRYSIAA
0omFruYxIp22cptvcnofWODzNOvu04Qd0Ix+MS+aUMRxvZK8vPY1+k0vKReXzL4f
KQJgHJI+eOCQwkipgKsXvFMDIg1ncZBuQg/6Pt1sHeWVj7F3BZA0csWNAArjuVsG
BAIVNjxwzo6dICJrZjRUPuOEfIQFNVJ0Kq0zQIhZ4sf4/CgJLoJ4H8CMqUan9Mcw
fJSL2f9dSzzinz3VUCmAytZ1+W42Yx7IzM6OkduroR4DKkZALy3ulyJMdlN3Q72T
KwIDAQAB
-----END PUBLIC KEY-----
";
    private readonly string _pkcs8Password = "123456";

    [TestMethod]
    public void EncryptedPkcs8DecryptStringTest()
    {
        var encrypted = "gfBOJsoxAm1uvhTtZ4toXJOTKdV5nz2WtfzbMBTeMU7RgscBTYTpyAADVyX0ttzoDRZwbRV9c/eUzX7hBKLIvU1vp/W+P/+NapqR2pXHO5VeltwxZ3WUnogtphlJuhfRQpSxNGqQUFlYtjya1xes2guvPZ3ul10rNif3khyHJ6NAGo40A+098EHlZ8HhudmZJjpeLGuDkX1TlhbHo4HzDOM0EzzFUAhlVzsdSLRMOOII3TqNCHuDWdBwq7eCQTG3j79jGtsrms/SIuQPtUTrArZ00OKe8b3ri/ZJfEkbDzTWLMai3IN6gUhVvksilb+7gRbXBPzGtaUYR+uh5ze2TA==";
        var expected = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Decrypt(encrypted, new RsaDecryptOption(_encryptedPkcs8Private) { PrivateKeyPassword = _pkcs8Password });
        var actual = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void EncryptedPkcs8EncryptStringTest()
    {
        var expected = "gfBOJsoxAm1uvhTtZ4toXJOTKdV5nz2WtfzbMBTeMU7RgscBTYTpyAADVyX0ttzoDRZwbRV9c/eUzX7hBKLIvU1vp/W+P/+NapqR2pXHO5VeltwxZ3WUnogtphlJuhfRQpSxNGqQUFlYtjya1xes2guvPZ3ul10rNif3khyHJ6NAGo40A+098EHlZ8HhudmZJjpeLGuDkX1TlhbHo4HzDOM0EzzFUAhlVzsdSLRMOOII3TqNCHuDWdBwq7eCQTG3j79jGtsrms/SIuQPtUTrArZ00OKe8b3ri/ZJfEkbDzTWLMai3IN6gUhVvksilb+7gRbXBPzGtaUYR+uh5ze2TA==";
        var plainText = "Hello,World";
        var bytes = _encryption.Asymmetric.Rsa.Encrypt(plainText, new RsaEncryptOption(_encryptedPkcs8Public));
        var actual = Convert.ToBase64String(bytes);
        Console.WriteLine(actual);
        //every result is different,cause secure random
        Assert.AreEqual(expected.Length, actual.Length);
    }

    [TestMethod]
    public void EncryptedPkcs8SignStringTest()
    {
        var plainText = "Hello,World";
        var expected = "9ngmelB5fF3VhTT2w//cxzACLvm82tyDvE3jXedXVnDOu2QRsFXy0JCkguZ/CHcrIjk/V7APzuUA6O8SFytha4pNWPwJDGC7GfaAtNdGdQFgfKMMf+rn70mImj8/nRteXcbu1gVhxwTwYjUFs8MiH+EIO8tvveM69UiooTMLlzdsi2SNos8skq86zqYsgwp83IjICgqFqxlSeI/QmOsEnZDketh+QQkNO++8iJ0YOX3inmXzncG5mErQ8KMz9GLEtw4tSZa7SsN2+3g7OD97NNOPu5VYAeSqhRcufwMIVFcy7e+bEr9lAUvGwHIVUn3jBPG0irrUfcZCLSGLHB8C/A==";
        var bytes = _encryption.Asymmetric.Rsa.Sign(plainText, new RsaSignOption(_encryptedPkcs8Private) { PrivateKeyPassword = _pkcs8Password });
        var actual = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void EncryptedPkcs8VerifySignStringTest()
    {
        var plainText = "Hello,World";
        var signature = "9ngmelB5fF3VhTT2w//cxzACLvm82tyDvE3jXedXVnDOu2QRsFXy0JCkguZ/CHcrIjk/V7APzuUA6O8SFytha4pNWPwJDGC7GfaAtNdGdQFgfKMMf+rn70mImj8/nRteXcbu1gVhxwTwYjUFs8MiH+EIO8tvveM69UiooTMLlzdsi2SNos8skq86zqYsgwp83IjICgqFqxlSeI/QmOsEnZDketh+QQkNO++8iJ0YOX3inmXzncG5mErQ8KMz9GLEtw4tSZa7SsN2+3g7OD97NNOPu5VYAeSqhRcufwMIVFcy7e+bEr9lAUvGwHIVUn3jBPG0irrUfcZCLSGLHB8C/A==";
        var isMatch = _encryption.Asymmetric.Rsa.VerifySign(plainText, new RsaVerifySignOption(Convert.FromBase64String(signature), _encryptedPkcs8Public));
        Assert.IsTrue(isMatch);
    }
    #endregion
}
