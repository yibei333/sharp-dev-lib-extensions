using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SharpDevLib.Extensions.Tests.Http;

[TestClass]
public class HttpTests
{
    private readonly IHttpService _http;

    public HttpTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddHttp();
        var serviceProvider = services.BuildServiceProvider();
        _http = serviceProvider.GetRequiredService<IHttpService>();
    }

    [TestMethod]
    [Ignore]
    public void Test()
    {
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes("wcf:password"));
        var headers = new Dictionary<string, string> { { "Authorization", $"Basic {token}" } };
        var option = new ParameterOption("http://ip:port/job/fx_wcf/lastSuccessfulBuild/artifact/file.zip") { Headers = headers, RetryCount = 3, OnReceiveProgress = (e) => Console.WriteLine(e.Progress) };
        var result = _http.GetAsync<string>(option).Result;
        Console.WriteLine(result.Serialize().FormatJson());
    }

    [TestMethod]
    [Ignore]
    public void GetStringTest()
    {
        var parameters = new { a = "1" }.Serialize().DeSerialize<Dictionary<string, string>>();
        var option = new ParameterOption("http://www.baidu1.com", parameters);
        var result = _http.GetAsync<string>(option).Result;
        Console.WriteLine(result.Serialize().FormatJson());
    }

    [TestMethod]
    [Ignore]
    public void GetBytesTest()
    {
        var url = "https://dss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo/logo_white-d0c9fe2af5.png";
        var option = new ParameterOption(url);
        var result = _http.GetAsync<byte[]>(option).Result;
        var target = "baidu.png";
        result.Data.SaveFile(target);
        Assert.IsTrue(File.Exists(target));
    }

    [TestMethod]
    [Ignore]
    public void PostFormTest()
    {
        var res = _http.PostFormAsync<string>(new FormOption("https://xxx.com/connect/token", new Dictionary<string, string>
        {
            { "client_id","game" },
            { "client_secret","123" },
            { "scope","openid profile offline_access test.api" },
            { "grant_type","password" },
            { "username","xx" },
            { "password","xx" },
        }, true)).Result;
        Console.WriteLine(res.Serialize().FormatJson());
        Assert.IsTrue(res.IsSuccess);
    }

    [Ignore]
    [TestMethod]
    public void PostTest()
    {
        var url = "https://localhost:7014/home/test";
        var option = new JsonOption(url);
        var res = _http.PostAsync<string>(option).Result;
        Debug.WriteLine(res);

        var url1 = "https://localhost:7014/home/test1";
        var option1 = new JsonOption(url1) { Cookies = res.Cookies };
        res = _http.PostAsync<string>(option1).Result;
        Debug.WriteLine(res);
    }

    [Ignore]
    [TestMethod]
    public void PostFormFilesTest()
    {
        var url = $"http://localhost:9090/api/file/upload?idpath=NzM0OGJlODQtNDZkOC00MDEzLThiMjUtZmY4OTI2YzgxOWFjOw";
        var files = new List<FormFile> { new("Files", "test.iso", new FileInfo(@"D:\迅雷下载\cn_sql_server_2014_enterprise_edition_with_service_pack_3_x64_dvd_4536733e.iso").OpenRead()) };
        var option = new FormOption(url, files)
        {
            OnSendProgress = (e) =>
            {
                Debug.WriteLine(e.ProgressString);
            }
        };
        var res = _http.PostFormAsync<string>(option).Result;
        Debug.WriteLine(res.Serialize().FormatJson());
    }

    [Ignore]
    [TestMethod]
    public async Task DownloadFilesTest()
    {
        var url = $"http://localhost:9090/api/foreverRecord/file?signature=4c5a1965c45eb5ef815d62c4076189a2e6e48ae997bdf3507c6374ef34e54d70";
        var option = new ParameterOption(url)
        {
            Headers=new Dictionary<string, string>
            {
                { "Authorization","" },
                { "aa","bb" },
            },
            OnReceiveProgress = (e) =>
            {
                Debug.WriteLine(e.ProgressString);
            }
        };
        var res = await _http.GetStreamAsync(option);
        using var fileStream = new FileStream(@"D:\gettest.iso", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        res.CopyTo(fileStream);
        fileStream.Flush();
        fileStream.Close();
    }
}
