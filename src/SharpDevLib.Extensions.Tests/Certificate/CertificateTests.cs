using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace SharpDevLib.Extensions.Certificate.Tests
{
    [TestClass()]
    public class CertificateTests
    {
        private readonly ICertificate _certificate;
        private readonly IRsaKey _rsaKey;

        public CertificateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddEncryption();
            services.AddCertificate();
            var provider = services.BuildServiceProvider();
            _certificate = provider.GetRequiredService<ICertificate>();
            _rsaKey = provider.GetRequiredService<IRsaKey>();
        }

        [TestMethod()]
        public void GenerateCATest()
        {
            var privateKey = _rsaKey.ExportPrivateKey(RsaKeyType.Pkcs8);
            var option = new CACertOption(privateKey, new SubjectOption("TestCA") { Organization = "abc", Country = "CN" });
            var certificate = _certificate.GenerateCA(option);

            File.WriteAllText("ca.key", privateKey);
            var crtPath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("ca.crt");
            certificate.SaveCrt(crtPath);

            var pfxPath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("ca.pfx");
            certificate.Export(X509ContentType.Pfx).SaveFile(pfxPath);

            Assert.IsTrue(File.Exists(crtPath));
            Assert.IsTrue(File.Exists(pfxPath));
        }

        [TestMethod()]
        public void GenerateChildCATest()
        {
            var caPrivateKey = File.ReadAllText("TestData/ca.key");
            var caCertificate = new X509Certificate2(X509Certificate.CreateFromCertFile("TestData/ca.crt"));

            var privateKey = _rsaKey.ExportPrivateKey(RsaKeyType.Pkcs8);
            var option = new CACertOption(caCertificate, caPrivateKey, privateKey, new SubjectOption("TestCA_Child"));

            var certificate = _certificate.GenerateCA(option);

            var crtPath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("ca_child.crt");
            certificate.Export(X509ContentType.Cert).SaveFile(crtPath);
            Assert.IsTrue(File.Exists(crtPath));
        }

        [TestMethod()]
        public void GenerateClientCertTest()
        {
            var caPrivateKey = File.ReadAllText("TestData/ca.key");
            var caCertificate = new X509Certificate2(X509Certificate.CreateFromCertFile("TestData/ca.crt"));

            var privateKey = _rsaKey.ExportPrivateKey(RsaKeyType.Pkcs8);
            var option = new ClientCertOption(caCertificate, caPrivateKey, privateKey, new SubjectOption("TestClient"), DateTime.Now.AddDays(-1), DateTime.Now.AddYears(1));
            File.WriteAllText("client.key", privateKey);

            var certificate = _certificate.GenerateClientCert(option);

            var crtPath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("client.crt");
            certificate.SaveCrt(crtPath);
            Assert.IsTrue(File.Exists(crtPath));
        }

        [TestMethod()]
        public void GenerateServerCertTest()
        {
            var caPrivateKey = File.ReadAllText("TestData/ca.key");
            var caCertificate = new X509Certificate2(X509Certificate.CreateFromCertFile("TestData/ca.crt"));

            var privateKey = _rsaKey.ExportPrivateKey(RsaKeyType.Pkcs8);
            var subjectAlternativeNames = new List<SubjectAlternativeNameOption>
            {
                new SubjectAlternativeNameOption(SubjectAlternativeNameType.Dns,"localhost"),
                new SubjectAlternativeNameOption(SubjectAlternativeNameType.Dns,"server"),
                new SubjectAlternativeNameOption(SubjectAlternativeNameType.IP,"127.0.0.1"),
                new SubjectAlternativeNameOption(SubjectAlternativeNameType.IP,"192.168.1.1"),
            };
            var option = new ServerCertOption(caCertificate, caPrivateKey, privateKey, new SubjectOption("TestServer"), subjectAlternativeNames);

            var certificate = _certificate.GenerateServerCert(option);

            var crtPath = AppDomain.CurrentDomain.BaseDirectory.CombinePath("server.crt");
            certificate.Export(X509ContentType.Cert).SaveFile(crtPath);
            Assert.IsTrue(File.Exists(crtPath));
        }
    }
}