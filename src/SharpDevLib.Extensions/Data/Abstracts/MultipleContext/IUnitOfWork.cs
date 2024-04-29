using Microsoft.EntityFrameworkCore;

namespace SharpDevLib.Extensions.Data;

/// <summary>
/// data unit of work abstraction
/// </summary>
/// <typeparam name="TDbContext">dbcontext type</typeparam>
public interface IUnitOfWork<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    /// begin db transaction
    /// </summary>
    /// <returns></returns>
    void BeginTransaction();

    /// <summary>
    /// save data changes
    /// </summary>
    /// <returns>number of affected rows</returns>
    void Commit();
}