using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork;

public interface IDatabaseContext : IDisposable
{
    ITransactionAdapter Transaction { get; }
    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    void CommitTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    void RollbackTransaction();
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}