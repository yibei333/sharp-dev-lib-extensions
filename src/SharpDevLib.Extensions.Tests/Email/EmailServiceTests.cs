using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharpDevLib.Extensions.Email.Tests;

[TestClass]
[Ignore]
public class EmailServiceTests
{
    private readonly IEmailService _emailService;
    private readonly string _rightSender = "aa@hotmail.com";
    private readonly string _rightSenderPassword = "aa";
    private readonly string _rightReceiver = "aa@qq.com";

    public EmailServiceTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddEmail(x =>
        {
            //x.Host = "smtp.qq.com";
            //x.Port = 587;
            x.Host = "smtp-mail.outlook.com";
            x.Port = 587;
            x.Sender = _rightSender;
            x.SenderPassword = _rightSenderPassword;
            x.UseSSL = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
    }

    [TestMethod]
    public void SendTest()
    {
        _emailService.Send(new EmailContent(new List<string> { _rightReceiver }, "test subject1", "test body"));
    }

    [TestMethod]
    public void SentFileTest()
    {
        _emailService.Send(new EmailContent(new List<string> { _rightReceiver }, "test file", "test body")
        {
            Attachments = new List<EmailAttachment>
            {
                new EmailAttachment(AppDomain.CurrentDomain.BaseDirectory.CombinePath("TestData/TestFile.txt"))
            }
        });
    }

    [TestMethod]
    public void SentBytesTest()
    {
        _emailService.Send(new EmailContent(new List<string> { _rightReceiver }, "test bytes", "test body")
        {
            Attachments = new List<EmailAttachment>
            {
                new EmailAttachment("test.txt",Encoding.UTF8.GetBytes("this is from bytes"))
            }
        });
    }

    [TestMethod]
    [ExpectedException(typeof(EmailVerifyException))]
    public void SendExceptionTest()
    {
        _emailService.Send(new EmailContent(new List<string> { }, "test subject", "test body"));
    }

    [TestMethod]
    public async Task SendAsyncTest()
    {
        await _emailService.SendAsync(new EmailContent(new List<string> { _rightReceiver }, "test subject2", "test body"), null);
        Console.WriteLine("ok");
    }
}
