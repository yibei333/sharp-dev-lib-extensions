using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Encryption;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpDevLib.Extensions.Tests.Encryption;

[TestClass]
public class AesTests : EncryptionTestBase
{
    [TestMethod]
    public void DecryptStringTest()
    {
        var encrypted = "aHQWQyT2/NdOST/cknnBBg==";
        var expected = "Hello,World";
        var key = "123456";
        byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");
        var bytes = _encryption.Symmetric.Aes.Decrypt(encrypted, new AesDecryptOption(key, iv));
        var decrypted = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, decrypted);
    }

    [TestMethod]
    public void DecryptFileTest()
    {
        var expected = "hello,this is from sharp dev lib extension unit test";
        var encryptedFile = "TestData/TestFile_AESEncrypted.txt";
        var decryptedFile = "TestData/TestFile_AESDecrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("0000000000000000");
        _encryption.Symmetric.Aes.DecryptFile(encryptedFile, decryptedFile, new AesDecryptOption(key, iv));
        var bytes = File.ReadAllBytes(decryptedFile);
        Assert.AreEqual(expected, Encoding.Default.GetString(bytes));
    }

    [TestMethod]
    public void EncryptStringTest()
    {
        var plainText = "Hello,World";
        var expected = "aHQWQyT2/NdOST/cknnBBg==";
        var key = "123456";
        byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");
        var bytes = _encryption.Symmetric.Aes.Encrypt(plainText, new AesEncryptOption(key, iv));
        var encrypted = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, encrypted);
    }

    [TestMethod]
    public void EncryptFileTest()
    {
        var file = "TestData/TestFile.txt";
        var encryptedFile = "TestFile_AESEncrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("0000000000000000");
        _encryption.Symmetric.Aes.EncryptFile(file, encryptedFile, new AesDecryptOption(key, iv));
        Assert.IsTrue(File.Exists(encryptedFile));
    }

    [TestMethod]
    public void GenerateIVTest()
    {
        var iv = _encryption.Symmetric.Aes.GenerateIV();
        Console.WriteLine(iv.ToList().Serialize());
        Assert.AreEqual(16, iv.Length);
    }
}
