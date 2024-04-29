using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharpDevLib.Extensions.Data.Implemention;

internal class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    public UnitOfWork(IServiceProvider serviceProvider)
    {
        serviceProvider.EnsureIsSingleContext();
        _dbContext = (serviceProvider.GetRequiredService<IDbContext>() as DbContext) ?? throw new Exception("your DbContext should implement service 'SharpDevLib.Data.IDbContext'");
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