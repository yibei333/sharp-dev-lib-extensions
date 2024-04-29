using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Encryption;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpDevLib.Extensions.Tests.Encryption;

[TestClass]
public class TripleDesTests : EncryptionTestBase
{
    [TestMethod]
    public void DecryptStringTest()
    {
        var encrypted = "o11RbRfEh2ZqHPgbJajIIg==";
        var expected = "Hello,World";
        var key = "123456789012345678901234";
        byte[] iv = Encoding.UTF8.GetBytes("12345678");
        var bytes = _encryption.Symmetric.TripleDes.Decrypt(encrypted, new TripleDesDecryptOption(key, iv));
        var decrypted = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, decrypted);
    }

    [TestMethod]
    public void DecryptFileTest()
    {
        var expected = "hello,this is from sharp dev lib extension unit test";
        var encryptedFile = "TestData/TestFile_TripleDesEncrypted.txt";
        var decryptedFile = "TestData/TestFile_TripleDesDecrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("00000000");
        _encryption.Symmetric.TripleDes.DecryptFile(encryptedFile, decryptedFile, new TripleDesDecryptOption(key, iv));
        var bytes = File.ReadAllBytes(decryptedFile);
        Assert.AreEqual(expected, Encoding.Default.GetString(bytes));
    }

    [TestMethod]
    public void EncryptStringTest()
    {
        var plainText = "Hello,World";
        var expected = "o11RbRfEh2ZqHPgbJajIIg==";
        var key = "123456789012345678901234";
        byte[] iv = Encoding.UTF8.GetBytes("12345678");
        var bytes = _encryption.Symmetric.TripleDes.Encrypt(plainText, new TripleDesEncryptOption(key, iv));
        var encrypted = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, encrypted);
    }

    [TestMethod]
    public void EncryptFileTest()
    {
        var file = "TestData/TestFile.txt";
        var encryptedFile = "TestFile_TripleDesEncrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("00000000");
        _encryption.Symmetric.TripleDes.EncryptFile(file, encryptedFile, new TripleDesDecryptOption(key, iv));
        Assert.IsTrue(File.Exists(encryptedFile));
    }

    [TestMethod]
    public void GenerateIVTest()
    {
        var iv = _encryption.Symmetric.TripleDes.GenerateIV();
        Console.WriteLine(iv.ToList().Serialize());
        Assert.AreEqual(8, iv.Length);
    }
}
