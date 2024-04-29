using Microsoft.EntityFrameworkCore;
using SharpDevLib.Extensions.Data;
using System;
using System.Linq;

namespace SharpDevLib.Extensions.Tests.Data;

public class FooDbContext : DbContext, IDbContext
{
    public FooDbContext(DbContextOptions<FooDbContext> options) : base(options)
    {

    }

    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<User> Users { get; set; }
}

public class Favorite : BaseEntity
{
    public string Thing { get; set; }
}

public class User : BaseEntity
{
    public User(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}

public class FooMigration : DataMigration<FooDbContext>
{
    public FooMigration(FooDbContext dbContext) : base(dbContext)
    {
    }

    public override void Seed()
    {
    }
}

public class UserRepository : Repository<User, FooDbContext>
{
    public UserRepository(FooDbContext dbContext) : base(dbContext)
    {
    }

    public override IQueryable<User> GetAll()
    {
        Console.WriteLine("user yes");
        return base.GetAll();
    }
}