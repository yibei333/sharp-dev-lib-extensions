using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Excel;
using SharpDevLib.Extensions.Excel.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace SharpDevLib.Extensions.Tests.Excel;

[TestClass]
public class ExcelTests
{
    private readonly IExcelService _excelService;
    private readonly IServiceProvider _serviceProvider;
    public ExcelTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddExcel();
        _serviceProvider = services.BuildServiceProvider();
        _excelService = _serviceProvider.GetRequiredService<IExcelService>();
    }

    private class TestModel : IExcelId
    {
        public TestModel()
        {
            Id = Guid.NewGuid();
            ExcelId = Guid.NewGuid();
        }

        [NotMapped()]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public DateTime CreateTime { get; set; }

        public Guid ExcelId { get; set; }
    }

    internal class TestTranslate : ITableTranslateService
    {
        public string[] Get(params string[] keys)
        {
            var result = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                result[i] = key == "Name" ? "名字" : key;
            }
            return result;
        }
    }

    internal class CustomHeadStyle : CellStyle
    {
        public CustomHeadStyle()
        {
            BackgroundColor = "315efb";
            Color = "ffffff";
            Bold = true;
            FontSize = 14;
            Italic = true;
        }
    }

    internal class CustomOddStyle : CellStyle
    {
        public CustomOddStyle()
        {
            Color = "000000";
        }
    }

    internal class CustomEvenStyle : CellStyle
    {
        public CustomEvenStyle()
        {
            BackgroundColor = "FFFFFF";
        }
    }

    [TestMethod]
    public void WriteTest()
    {
        var data1 = new List<TestModel> {
            new TestModel{Name="jarvan",Age=10,Salary=1.1m,CreateTime=DateTime.Now},
            new TestModel{Name="lee",Age=11,Salary=2.1m,CreateTime=DateTime.Now},
            new TestModel{Name="jax",Age=12,Salary=3.1m,CreateTime=DateTime.Now},
        };
        var table1 = data1.ToTable(new TestTranslate());
        var data2 = data1.DeepClone();
        data2.Add(new TestModel { Name = "yi", Age = 18, Salary = 100m, CreateTime = new DateTime(2022, 5, 1) });
        var table2 = data2.ToTable(new TestTranslate());

        var filePath = $"employee123.xlsx";
        var option1 = new ExcelWriteOptions(table1, new List<ExcelRemark> { new ExcelRemark(data1.First(x => x.Name == "lee").ExcelId, new TestTranslate().Get("Name").First(), "Hello", "jinx") });
        option1.FileRemark.Content = "xxxx,this is description,vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv";
        //option1.FileRemark.ColSpan = 4;
        option1.FileRemark.RowSpan = 4;

        var option2 = new ExcelWriteOptions(table2, new List<ExcelRemark> {
            new ExcelRemark(data2.First(x => x.Name == "jax").ExcelId, new TestTranslate().Get("Salary").First(), "Nope", "jinx"),
            new ExcelRemark(data2.First(x => x.Name == "jax").ExcelId, new TestTranslate().Get("Salary").First(), "rrrr", "jinx"),
            new ExcelRemark(data2.First(x => x.Name == "jax").ExcelId, new TestTranslate().Get("Name").First(), "yeah", "jinx")
        });
        //option2.ExcelStyles.ClearStyle = true;
        option2.ExcelStyles.Head = typeof(CustomHeadStyle);
        option2.ExcelStyles.Rows = new List<RowStyleOption> {
            new RowStyleOption(data2.First().ExcelId,typeof(CustomEvenStyle)),
            new RowStyleOption(data2.Last().ExcelId,typeof(CustomOddStyle))
        };
        option2.ExcelStyles.Cols = new List<ColStyleOption> {
            new ColStyleOption(new TestTranslate().Get("Name").First(),typeof(ErrorStyle)),
            new ColStyleOption("CreateTime",typeof(WarningStyle))
        };
        option2.ExcelStyles.Cells = new List<CellStyleOption> {
            new CellStyleOption(data2.First().ExcelId,"Age",typeof(HeadStyle)),
            new CellStyleOption(data2.Last().ExcelId,"Age",typeof(WarningStyle)),
        };
        option2.FileRemark.Content = "hello,this is description,vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv";
        //option2.FileRemark.ColSpan = 4;
        option2.FileRemark.RowSpan = 4;

        var bytes = _excelService.Write(option1, option2);
        bytes.SaveFile(filePath);
        Assert.IsTrue(File.Exists(filePath));
    }

    [TestMethod]
    public void WriteWithProtectedTest()
    {
        var data1 = new List<TestModel> {
            new TestModel{Name="jarvan",Age=10,Salary=1.1m,CreateTime=DateTime.Now},
            new TestModel{Name="lee",Age=11,Salary=2.1m,CreateTime=DateTime.Now},
            new TestModel{Name="jax",Age=12,Salary=3.1m,CreateTime=DateTime.Now},
        };
        var table1 = data1.ToTable(new TestTranslate());
        var data2 = data1.DeepClone();
        data2.Add(new TestModel { Name = "yi", Age = 18, Salary = 100m, CreateTime = new DateTime(2022, 5, 1) });
        var table2 = data2.ToTable(new TestTranslate());

        var filePath = $"employee-encrypted.xlsx";
        var option1 = new ExcelWriteOptions(table1, new List<ExcelRemark> { new ExcelRemark(data1.First(x => x.Name == "lee").ExcelId, new TestTranslate().Get("Name").First(), "Hello", "jinx") });
        var option2 = new ExcelWriteOptions(table2, new List<ExcelRemark> {
            new ExcelRemark(data2.First(x => x.Name == "jax").ExcelId, new TestTranslate().Get("Salary").First(), "Nope", "jinx"),
            new ExcelRemark(data2.First(x => x.Name == "jax").ExcelId, new TestTranslate().Get("Salary").First(), "rrrr", "jinx")
        });
        //option2.ExcelStyles.ClearStyle = true;
        option2.ExcelStyles.Head = typeof(CustomHeadStyle);
        option2.ExcelStyles.Rows = new List<RowStyleOption> {
            new RowStyleOption(data2.First().ExcelId,typeof(CustomEvenStyle)),
            new RowStyleOption(data2.Last().ExcelId,typeof(CustomOddStyle))
        };
        option2.ExcelStyles.Cols = new List<ColStyleOption> {
            new ColStyleOption(new TestTranslate().Get("Name").First(),typeof(ErrorStyle)),
            new ColStyleOption("CreateTime",typeof(WarningStyle))
        };
        option2.ExcelStyles.Cells = new List<CellStyleOption> {
            new CellStyleOption(data2.First().ExcelId,"Age",typeof(HeadStyle)),
            new CellStyleOption(data2.Last().ExcelId,"Age",typeof(WarningStyle)),
        };

        var bytes = _excelService.Write("123", option1, option2);
        bytes.SaveFile(filePath);
        Assert.IsTrue(File.Exists(filePath));
    }

    [TestMethod]
    public void ReadTest()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory.CombinePath("TestData/read.xlsx");
        var tables = _excelService.Read(path, new ExcelReadOptions(1), new ExcelReadOptions(0));
        Assert.AreEqual(2, tables.Length);
        Assert.AreEqual(4, tables[0].Rows.Count);
        Assert.AreEqual(3, tables[1].Rows.Count);

        var list1 = tables[0].ToList<TestModel>(new TestTranslate());
        var list2 = tables[1].ToList<TestModel>(new TestTranslate());
        Console.WriteLine(list1.Serialize().FormatJson());
        Console.WriteLine(list2.Serialize().FormatJson());
        Assert.AreEqual(4, list1.Count);
        Assert.AreEqual(3, list2.Count);
    }

    [TestMethod]
    public void ReadWithProtectionTest()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory.CombinePath("TestData/read-protected.xlsx");
        var tables = _excelService.Read(path, "123", new ExcelReadOptions(1), new ExcelReadOptions(0));
        //Assert.AreEqual(2, tables.Length);
        //Assert.AreEqual(4, tables[0].Rows.Count);
        //Assert.AreEqual(3, tables[1].Rows.Count);

        var list1 = tables[0].ToList<TestModel>(new TestTranslate());
        var list2 = tables[1].ToList<TestModel>(new TestTranslate());
        Console.WriteLine(list1.Serialize().FormatJson());
        Console.WriteLine(list2.Serialize().FormatJson());
        Assert.AreEqual(4, list1.Count);
        Assert.AreEqual(3, list2.Count);
    }

    internal class NameModel : IExcelId
    {
        public string Name { get; set; }
        public Guid ExcelId { get; set; }

        public NameModel()
        {
            ExcelId = Guid.NewGuid();
        }
        public NameModel(string name) : this()
        {
            Name = name;
        }
    }

    [TestMethod]
    public void Test()
    {
        var model = new List<NameModel>
        {
                new NameModel("foo"),
                new NameModel("bar"),
        };
        _excelService.Write(new ExcelWriteOptions(model.ToTable())).SaveFile("a.xlsx");

        var encryptOption = new ExcelWriteOptions(model.ToTable());
        //encryptOption.ExcelStyles.ClearStyle = true;
        _excelService.Write("123", encryptOption).SaveFile("b.xlsx");

        var other = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IExcelService>();
        var xtable = other.Read("b.xlsx", "123", new ExcelReadOptions(0)).First();
        var xmodel = xtable.ToList<NameModel>();
        Console.WriteLine(xmodel.Serialize().FormatJson());
    }

    [TestMethod]
    [Ignore]
    public void ValidateTest()
    {
        var validator = new OpenXmlValidator();
        var errors = validator.Validate(SpreadsheetDocument.Open("employee.xlsx", false)).ToList();

        int count = 0;
        foreach (ValidationErrorInfo error in errors)
        {
            count++;
            Console.WriteLine("Error " + count);
            Console.WriteLine("Description: " + error.Description);
            Console.WriteLine("ErrorType: " + error.ErrorType);
            Console.WriteLine("Node: " + error.Node);
            Console.WriteLine("Path: " + error.Path?.XPath);
            Console.WriteLine("Part: " + error.Part?.Uri);
            Console.WriteLine("-------------------------------------------");
        }
    }

    [TestMethod]
    [Ignore]
    public void BinaryTest()
    {
        var byteCorrect = File.ReadAllBytes("a.xlsx");
        var byteError = File.ReadAllBytes("r2.txt");

        var countEqual = byteCorrect.Length == byteError.Length;
        Console.WriteLine(byteError.Length);
        Console.WriteLine(countEqual);
        for (int i = 0; i < byteCorrect.Length; i++)
        {
            if (byteCorrect[i] != byteError[i])
            {
                //Console.WriteLine(i);
            }
        }
    }

    [TestMethod]
    public void CoverTest()
    {
        var origin = @"TestData/origin.xlsx";
        var encrypted = "encrypted.xlsx";
        var decrypted = "decrypted.xlsx";
        _excelService.Encrypt(origin, "123", encrypted);
        _excelService.Decrypt(encrypted, "123", decrypted);
    }
}