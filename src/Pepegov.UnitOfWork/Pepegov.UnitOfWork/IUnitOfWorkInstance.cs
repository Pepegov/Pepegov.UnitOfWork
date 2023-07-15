using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkInstance : IDisposable
{
    IDatabaseContext DatabaseContext { get; }
    public SaveChangesResult? LastSaveChangesResult { get; }

    void BeginTransaction();
    Task BeginTransactionAsync();
    
    void CommitTransaction();
    Task CommitTransactionAsync();
    
    void RollbackTransaction();
    Task RollbackTransactionAsync();
}