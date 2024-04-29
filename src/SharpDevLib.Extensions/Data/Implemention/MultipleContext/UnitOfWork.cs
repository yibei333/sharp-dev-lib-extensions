using Microsoft.EntityFrameworkCore;

namespace SharpDevLib.Extensions.Data.Implemention;

internal class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public UnitOfWork(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void BeginTransaction()
    {
        if (_dbContext.Database.CurrentTransaction != null) return;
        _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.Database.CurrentTransaction?.Commit();
    }
}