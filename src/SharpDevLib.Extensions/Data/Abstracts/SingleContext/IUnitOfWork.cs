namespace SharpDevLib.Extensions.Data;

/// <summary>
/// data unit of work abstraction
/// </summary>
public interface IUnitOfWork
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