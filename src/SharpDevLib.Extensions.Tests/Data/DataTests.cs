using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using SharpDevLib.Extensions.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

namespace SharpDevLib.Extensions.Tests.Data;

[TestClass]
public class DataTests
{
    [TestMethod]
    public void SingleDbContextTest()
    {
        File.Delete(DesignConstraint.SingleFooConnectionFile);
        IServiceCollection services = new ServiceCollection();
        services.AddData<FooDbContext, FooMigration>(x => x.UseSqlite(DesignConstraint.SingleFooConnectionString));
        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<DataMigration>().Migrate();

        var users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, users.Count);

        provider.GetRequiredService<IRepository<User>>().Add(new User("abc"));
        provider.GetRequiredService<ISqlRepository>().ExecuteNoneQuery($"INSERT INTO [Users](Id,Name,CreateTime) VALUES('{Guid.NewGuid()}','jarvan','{DateTime.Now.ToUtcTimestamp()}')");

        users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        var fRepository = provider.GetRequiredService<IRepository<Favorite>>();
        var favorites = fRepository.GetAll().ToListAsync().Result;
        Console.WriteLine(favorites.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, favorites.Count);

        fRepository.Add(new Favorite() { Thing = "Favoritexyz" });
        favorites = fRepository.GetAll().ToListAsync().Result;
        Console.WriteLine(favorites.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(1, favorites.Count);

        Assert.AreEqual(2, provider.CreateScope().ServiceProvider.GetRequiredService<ISqlRepository>().ExecuteScalar<int>($"SELECT COUNT(1) FROM [Users]"));
        Assert.AreEqual("abc", provider.CreateScope().ServiceProvider.GetRequiredService<ISqlRepository>().ExecuteScalar<string>($"SELECT [Name] FROM [Users] ORDER BY CreateTime"));
    }

    [TestMethod]
    public void MultipleDbContextTest()
    {
        File.Delete(DesignConstraint.MultipleFooConnectionFile);
        File.Delete(DesignConstraint.MultipleBarConnectionFile);
        IServiceCollection services = new ServiceCollection();
        services.AddData<FooDbContext, FooMigration>(x => x.UseSqlite(DesignConstraint.MultipleFooConnectionString));
        services.AddData<BarDbContext, BarMigration>(x => x.UseSqlite(DesignConstraint.MultipleBarConnectionString));
        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<DataMigration<FooDbContext>>().Migrate();
        provider.GetRequiredService<DataMigration<BarDbContext>>().Migrate();

        var users = provider.GetRequiredService<IRepository<User, FooDbContext>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, users.Count);

        provider.GetRequiredService<IRepository<User, FooDbContext>>().Add(new User("abc"));
        provider.GetRequiredService<ISqlRepository<FooDbContext>>().ExecuteNoneQuery($"INSERT INTO [Users](Id,Name,CreateTime) VALUES('{Guid.NewGuid()}','jarvan','{DateTime.Now.ToUtcTimestamp()}')");

        users = provider.GetRequiredService<IRepository<User, FooDbContext>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User, FooDbContext>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

      
        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User, FooDbContext>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.GetRequiredService<IRepository<User, FooDbContext>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        var fRepository = provider.GetRequiredService<IRepository<Favorite, FooDbContext>>();
        var favorites = fRepository.GetAll().ToListAsync().Result;
        Console.WriteLine(favorites.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, favorites.Count);

        fRepository.Add(new Favorite() { Thing = "Favoritexyz" });
        favorites = fRepository.GetAll().ToListAsync().Result;
        Console.WriteLine(favorites.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(1, favorites.Count);

        Assert.AreEqual(2, provider.CreateScope().ServiceProvider.GetRequiredService<ISqlRepository<FooDbContext>>().ExecuteScalar<int>($"SELECT COUNT(1) FROM [Users]"));
        Assert.AreEqual("abc", provider.CreateScope().ServiceProvider.GetRequiredService<ISqlRepository<FooDbContext>>().ExecuteScalar<string>($"SELECT [Name] FROM [Users] ORDER BY CreateTime"));
    }

    [TestMethod]
    public void UnitOfWorkTest()
    {
        File.Delete(DesignConstraint.SingleBarConnectionFile);
        IServiceCollection services = new ServiceCollection();
        services.AddData<BarDbContext, BarMigration>(x => x.UseSqlite(DesignConstraint.SingleBarConnectionString));
        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<DataMigration>().Migrate();

        var users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, users.Count);

        IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        unitOfWork.BeginTransaction();

        provider.GetRequiredService<IRepository<User>>().Add(new User("abc"));
        provider.GetRequiredService<ISqlRepository>().ExecuteNoneQuery($"INSERT INTO [Users](Id,Name,CreateTime) VALUES('{Guid.NewGuid()}','jarvan','{DateTime.Now.ToUtcTimestamp()}')");

        users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(0, users.Count);

        unitOfWork.Commit();
        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);

        users = provider.GetRequiredService<IRepository<User>>().GetAll().ToListAsync().Result;
        Console.WriteLine(users.Serialize().FormatJson());
        Console.WriteLine("-------------");
        Assert.AreEqual(2, users.Count);
    }
}
