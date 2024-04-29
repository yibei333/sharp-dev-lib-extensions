using Microsoft.EntityFrameworkCore;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// data migration abastraction
/// </summary>
/// <typeparam name="TDbContext">dbcontext type</typeparam>
public abstract class DataMigration<TDbContext> : DataMigration where TDbContext : DbContext
{
    /// <summary>
    /// instantient data migration instance
    /// </summary>
    /// <param name="dbContext">dbcontext</param>
    public DataMigration(TDbContext dbContext) : base(dbContext)
    {
    }
}

/// <summary>
/// data migration abastraction
/// </summary>
public abstract class DataMigration
{
    /// <summary>
    /// dbcontext instance
    /// </summary>
    protected readonly DbContext _dbContext;

    /// <summary>
    /// instantient data migration instance
    /// </summary>
    /// <param name="dbContext">dbcontext abstract</param>
    public DataMigration(IDbContext dbContext)
    {
        _dbContext = dbContext as DbContext ?? throw new Exception("your DbContext should implement service 'SharpDevLib.Data.IDbContext'");
    }

    /// <summary>
    /// instantient data migration instance
    /// </summary>
    /// <param name="dbContext">dbcontext</param>
    protected DataMigration(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// if have seed data,add to here
    /// </summary>
    public abstract void Seed();

    /// <summary>
    /// migrate database
    /// </summary>
    public void Migrate()
    {
        _dbContext.Database.Migrate();
        Seed();
    }
}
