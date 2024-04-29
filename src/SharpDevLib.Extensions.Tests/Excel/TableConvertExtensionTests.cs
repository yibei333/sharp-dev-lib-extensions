using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpDevLib.Extensions.Excel.Tests;

[TestClass]
public class TableConvertExtensionTests
{
    [TestMethod]
    public void ToTableTest()
    {
        foreach (var item in typeof(TestModel).GetProperties())
        {
            Console.WriteLine(item.Name);
        }

        var list = new List<Model1>
        {
            new Model1{ Id = 1, Name ="jarvan",Age=18,CreateTime=DateTime.Now.ToUtcTimestamp(),UpdateTime=DateTime.Now.AddMonths(1),DynamicModel2=new List<Model2>{ new Model2("col1") { Address="cd",Height=120,Id=Guid.NewGuid(),Name="col1" } },ComplexModel3=new Model3{ Id=111,Price=1.1m,Salary=1.2m } },
            new Model1{ Id = 2, Name ="jax",Age=28},
        };
        var dynamics = new List<IdNameModel> {
            new IdNameModel("col1"),
            new IdNameModel("col2"),
            new IdNameModel("col3"),
        };
        var table = list.ToTable(new[] { dynamics });
        Assert.AreEqual(12, table.Columns.Count);
    }

    [TestMethod]
    public void ToListTest()
    {
        var list = new List<Model1>
        {
            new Model1{ Id = 1, Name ="jarvan",Age=18,CreateTime=DateTime.Now.ToUtcTimestamp(),UpdateTime=DateTime.Now.AddMonths(1),DynamicModel2=new List<Model2>{ new Model2("col1") { Address="cd",Height=120,Id=Guid.NewGuid(),Name="col1" } },ComplexModel3=new Model3{ Id=111,Price=1.1m,Salary=1.2m } },
            new Model1{ Id = 2, Name ="jax",Age=28},
        };
        var dynamics = new List<IdNameModel> {
            new IdNameModel("col1"),
            new IdNameModel("col2"),
            new IdNameModel("col3"),
        };
        var table = list.ToTable(new[] { dynamics }, new TestTranslate());
        var resultList = table.ToList<Model1>(new[] { dynamics }, new TestTranslate());
        Console.WriteLine(list.Serialize().FormatJson());
        Console.WriteLine(resultList.Serialize().FormatJson());
        Assert.AreEqual(list.Count, resultList.Count);
    }

    #region TestData
    internal class TestModel
    {
        public int C { get; set; }
        public int A { get; set; }
        public int B { get; set; }
    }

    internal class Model1
    {
        [NotMapped]
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        [TableConvert(Name = "年龄")]
        public int Age { get; set; }
        [TableConvert(IsDynamic = true)]
        public List<Model2> DynamicModel2 { get; set; }
        public Model3 ComplexModel3 { get; set; }
        [TableConvert(DateFormat = "yyyy-MM-dd HH:mm:ss.fff zzz", TimestampToDateTime = true)]
        public long CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    internal class Model2 : IDynamicKey
    {
        public Model2()
        {
            DynamicName = string.Empty;
        }

        [NotMapped]
        [JsonIgnore]
        public Guid Id { get; set; }
        [NotMapped]
        public string DynamicName { get; set; }

        public Model2(string dynamicName)
        {
            DynamicName = dynamicName;
        }

        [NotMapped]
        [JsonIgnore]
        public string Name { get; set; }
        public string Address { get; set; }
        [TableConvert(DefaultValue = 10, Decimals = 1)]
        public double Height { get; set; }
    }

    internal class Model3
    {
        [NotMapped]
        [JsonIgnore]
        public int Id { get; set; }
        public decimal Price { get; set; }
        [TableConvert(DefaultValue = 0, Decimals = 3)]
        public decimal Salary { get; set; }
    }

    internal class IdNameModel : IDynamicKey
    {
        public string DynamicName { get; set; }

        public IdNameModel(string dynamicName)
        {
            DynamicName = dynamicName;
        }
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
    #endregion
}
