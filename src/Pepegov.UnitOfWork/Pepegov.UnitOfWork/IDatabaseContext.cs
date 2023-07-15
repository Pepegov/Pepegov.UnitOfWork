using System.Data;

namespace Pepegov.UnitOfWork;

public interface IDatabaseContext : IDisposable
{
    IDbTransaction Transaction { get; }
    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    void CommitTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    void RollbackTransaction();
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}