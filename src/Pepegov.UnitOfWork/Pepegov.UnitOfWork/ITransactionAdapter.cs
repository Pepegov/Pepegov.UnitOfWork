using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork;

public interface ITransactionAdapter : IDisposable
{
    public void BeginTransaction();
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    public void Commit();
    public Task CommitAsync(CancellationToken cancellationToken = default);

    public void Rollback();
    public Task RollbackAsync(CancellationToken cancellationToken = default);
}