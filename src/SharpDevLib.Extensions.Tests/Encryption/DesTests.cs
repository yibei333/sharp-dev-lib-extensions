using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Encryption;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpDevLib.Extensions.Tests.Encryption;

[TestClass]
public class DesTests : EncryptionTestBase
{
    [TestMethod]
    public void DecryptStringTest()
    {
        var encrypted = "UmLVto2eD+0LbeB94WI0cA==";
        var expected = "Hello,World";
        var key = "123456";
        byte[] iv = Encoding.UTF8.GetBytes("12345678");
        var bytes = _encryption.Symmetric.Des.Decrypt(encrypted, new DesDecryptOption(key, iv));
        var decrypted = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(expected, decrypted);
    }

    [TestMethod]
    public void DecryptFileTest()
    {
        var expected = "hello,this is from sharp dev lib extension unit test";
        var encryptedFile = "TestData/TestFile_DesEncrypted.txt";
        var decryptedFile = "TestData/TestFile_DesDecrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("00000000");
        _encryption.Symmetric.Des.DecryptFile(encryptedFile, decryptedFile, new DesDecryptOption(key, iv));
        var bytes = File.ReadAllBytes(decryptedFile);
        Assert.AreEqual(expected, Encoding.Default.GetString(bytes));
    }

    [TestMethod]
    public void EncryptStringTest()
    {
        var plainText = "Hello,World";
        var expected = "UmLVto2eD+0LbeB94WI0cA==";
        var key = "123456";
        byte[] iv = Encoding.UTF8.GetBytes("12345678");
        var bytes = _encryption.Symmetric.Des.Encrypt(plainText, new DesEncryptOption(key, iv));
        var encrypted = Convert.ToBase64String(bytes);
        Assert.AreEqual(expected, encrypted);
    }

    [TestMethod]
    public void EncryptFileTest()
    {
        var file = "TestData/TestFile.txt";
        var encryptedFile = "TestFile_DesEncrypted.txt";
        var key = "123";
        byte[] iv = Encoding.UTF8.GetBytes("00000000");
        _encryption.Symmetric.Des.EncryptFile(file, encryptedFile, new DesDecryptOption(key, iv));
        Assert.IsTrue(File.Exists(encryptedFile));
    }

    [TestMethod]
    public void GenerateIVTest()
    {
        var iv = _encryption.Symmetric.Des.GenerateIV();
        Console.WriteLine(iv.ToList().Serialize());
        Assert.AreEqual(8, iv.Length);
    }
}
