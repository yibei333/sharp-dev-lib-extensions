using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// base repository implemention
/// </summary>
/// <typeparam name="TEntity">entity type</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// dbset
    /// </summary>
    public DbSet<TEntity> Set => _dbSet;

    /// <summary>
    /// instantient repository
    /// </summary>
    /// <param name="serviceProvider">service provider</param>
    public Repository(IServiceProvider serviceProvider)
    {
        serviceProvider.EnsureIsSingleContext();
        _dbContext = (serviceProvider.GetRequiredService<IDbContext>() as DbContext) ?? throw new Exception("your DbContext should implement service 'SharpDevLib.Data.IDbContext'");
        _dbSet = _dbContext.Set<TEntity>();
    }

    /// <summary>
    /// get entity by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>entity</returns>
    public virtual TEntity? Get(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.FirstOrDefault(expression);
    }

    /// <summary>
    /// get entity by expression as no tracking
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>entity</returns>
    public virtual TEntity? GetAsNoTracking(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.Where(expression).AsNoTracking().FirstOrDefault();
    }

    /// <summary>
    /// get count by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>count</returns>
    public virtual int Count(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.Count(expression);
    }

    /// <summary>
    /// get is any element existed by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>is any element existed</returns>
    public virtual bool Any(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.Any(expression);
    }

    /// <summary>
    /// get many by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>many entities</returns>
    public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.Where(expression);
    }

    /// <summary>
    /// get all
    /// </summary>
    /// <returns>all entities</returns>
    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbSet.Where(x => true);
    }

    /// <summary>
    /// add entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// add entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// update entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// update entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// remove entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    public virtual void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// remove entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        _dbContext.SaveChanges();
    }
}