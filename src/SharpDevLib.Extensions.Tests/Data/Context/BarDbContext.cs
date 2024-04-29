using Microsoft.EntityFrameworkCore;
using SharpDevLib.Extensions.Data;

namespace SharpDevLib.Extensions.Tests.Data;

public class BarDbContext : DbContext, IDbContext
{
    public BarDbContext(DbContextOptions<BarDbContext> options) : base(options)
    {

    }

    public DbSet<BarSampleEntity> BarSamples { get; set; }
    public DbSet<User> Users { get; set; }
}

public class BarSampleEntity : BaseEntity
{
    public string Data { get; set; }
}

public class BarMigration : DataMigration<BarDbContext>
{
    public BarMigration(BarDbContext dbContext) : base(dbContext)
    {
    }

    public override void Seed()
    {
    }
}