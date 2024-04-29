using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// repository base abstraction
/// </summary>
/// <typeparam name="TEntity">entity type</typeparam>
/// <typeparam name="TDbContext">dbcontext type</typeparam>
public interface IRepository<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext
{
    /// <summary>
    /// dbcontext
    /// </summary>
    TDbContext DbContext { get; }

    /// <summary>
    /// dbset
    /// </summary>
    DbSet<TEntity> Set { get; }

    /// <summary>
    /// get entity by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>entity</returns>
    TEntity? Get(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// get entity by expression as no tracking
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>entity</returns>
    TEntity? GetAsNoTracking(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// get count by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>count</returns>
    int Count(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// get is any element existed by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>is any element existed</returns>
    bool Any(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// get many by expression
    /// </summary>
    /// <param name="expression">query expression</param>
    /// <returns>many entities</returns>
    IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> expression);

    /// <summary>
    /// get all
    /// </summary>
    /// <returns>all entities</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// add entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    void Add(TEntity entity);

    /// <summary>
    /// add entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// update entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    void Update(TEntity entity);

    /// <summary>
    /// update entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// remove entity to db
    /// </summary>
    /// <param name="entity">entity</param>
    void Remove(TEntity entity);

    /// <summary>
    /// remove entities to db
    /// </summary>
    /// <param name="entities">entity collection</param>
    void RemoveRange(IEnumerable<TEntity> entities);
}