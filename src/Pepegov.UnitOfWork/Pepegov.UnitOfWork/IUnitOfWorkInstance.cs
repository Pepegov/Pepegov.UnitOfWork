using System;
using System.Threading;
using System.Threading.Tasks;
using Pepegov.UnitOfWork.Entityes;

namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkInstance : IDisposable
{
    IDatabaseContext DatabaseContext { get; }
    public SaveChangesResult? LastSaveChangesResult { get; }

    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    void CommitTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    void RollbackTransaction();
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}