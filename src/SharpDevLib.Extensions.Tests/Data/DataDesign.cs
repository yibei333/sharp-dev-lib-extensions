using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace SharpDevLib.Extensions.Tests.Data;

public class DesignConstraint
{
    public static readonly string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;

    public static readonly string SingleFooConnectionFile = CurrentPath.CombinePath("TestData/sfoo.db");
    public static readonly string SingleFooConnectionString = $"data source={SingleFooConnectionFile}";
    public static readonly string SingleBarConnectionFile = CurrentPath.CombinePath("TestData/sbar.db");
    public static readonly string SingleBarConnectionString = $"data source={SingleBarConnectionFile}";

    public static readonly string MultipleFooConnectionFile = CurrentPath.CombinePath("TestData/mfoo.db");
    public static readonly string MultipleFooConnectionString = $"data source={MultipleFooConnectionFile}";
    public static readonly string MultipleBarConnectionFile = CurrentPath.CombinePath("TestData/mbar.db");
    public static readonly string MultipleBarConnectionString = $"data source={MultipleBarConnectionFile}";
}

public class FooDesign : IDesignTimeDbContextFactory<FooDbContext>
{
    public FooDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<FooDbContext>();
        builder.UseSqlite(DesignConstraint.SingleFooConnectionString);
        return new FooDbContext(builder.Options);
    }
}

public class BarDesign : IDesignTimeDbContextFactory<BarDbContext>
{
    public BarDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BarDbContext>();
        builder.UseSqlite(DesignConstraint.SingleBarConnectionString);
        return new BarDbContext(builder.Options as DbContextOptions<BarDbContext>);
    }
}