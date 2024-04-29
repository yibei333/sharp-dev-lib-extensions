using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Model;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SharpDevLib.Extensions.Data.Tests
{
    [TestClass]
    [Ignore]
    public class SqlHelperTests
    {
        private readonly IServiceProvider _serviceProvider;
        public SqlHelperTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSqlHelper(SqlClientFactory.Instance, "Data Source=127.0.0.1; Database=TestDB; User ID=sa; Password=12345678;");
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void TransactionTest()
        {
            var helper = _serviceProvider.CreateScope().ServiceProvider.GetService<ISqlHelper>();
            using var transaction = helper.BeginTransaction();
            var sql = $@"CREATE TABLE #Test(Id INT);INSERT INTO #Test VALUES(1),(2)";
            helper.ExecuteNonQuery(sql);
            transaction.Commit();
            var count = helper.ExecuteScalar<int>("SELECT COUNT(1) FROM #Test");
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void ExecuteScalarTest()
        {
            var helper = _serviceProvider.CreateScope().ServiceProvider.GetService<ISqlHelper>();
            var count = helper.ExecuteScalar<int>($"SELECT COUNT(1) FROM DataDictionary");
            Debug.WriteLine(count);
            Assert.IsTrue(count > 0);

            var bytes = helper.ExecuteScalar<byte[]>($"SELECT TOP 1 UpdateFlag FROM EmployeeEntryOrder");
            Debug.WriteLine(bytes.Base64Encode());
            Assert.IsTrue(bytes.NotEmpty());

            var boolValue = helper.ExecuteScalar<bool>($"SELECT CAST(1 AS BIT)");
            Assert.IsTrue(boolValue);
        }

        [TestMethod]
        public void ExecuteDataSetTest()
        {
            var helper = _serviceProvider.CreateScope().ServiceProvider.GetService<ISqlHelper>();
            var set = helper.ExecuteDataSet($"SELECT TOP 10 Id,Name FROM DataDictionary;SELECT TOP 10 Id,Name FROM Employee;");
            Assert.IsTrue(set.Tables.Count == 2);
        }

        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            var helper = _serviceProvider.CreateScope().ServiceProvider.GetService<ISqlHelper>();
            var sql = $@"CREATE TABLE #AA(Id INT);INSERT INTO #AA VALUES(1),(2)";
            helper.ExecuteNonQuery(sql);
            var count = helper.ExecuteScalar<int>("SELECT COUNT(1) FROM #AA");
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void QueryListTest()
        {
            var helper = _serviceProvider.CreateScope().ServiceProvider.GetService<ISqlHelper>();
            var list = helper.QueryList<IdNameDto>($"select * from DataDictionary where name like @level;SELECT TOP 10 Id,Name FROM Employee;", new SqlParameter("@level", "%等级%"));
            Debug.WriteLine(list.Serialize());
            Assert.IsTrue(list.Count == 4);

            var byteModelList = helper.QueryList<IdNameDataDto<byte[]>>($"SELECT Id,EmployeeName,UpdateFlag [Data] FROM EmployeeEntryOrder WHERE Remarks LIKE @remark;", new SqlParameter("@remark", "%申请%"));
            Debug.WriteLine(byteModelList.Serialize());
            Assert.IsTrue(byteModelList.Count == 18);
        }
    }
}