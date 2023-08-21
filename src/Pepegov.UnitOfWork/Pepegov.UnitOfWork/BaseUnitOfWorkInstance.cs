using System;
using System.Threading;
using System.Threading.Tasks;
using Pepegov.UnitOfWork.Entityes;

namespace Pepegov.UnitOfWork;

public abstract class BaseUnitOfWorkInstance : IUnitOfWorkInstance
{
    public IDatabaseContext DatabaseContext { get; }
    public SaveChangesResult? LastSaveChangesResult { get; protected set; }
    
    protected BaseUnitOfWorkInstance(IDatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
        LastSaveChangesResult = new SaveChangesResult();
    }

    public abstract void Dispose();
    
    #region Transaction

    public void BeginTransaction()
        => TryTransaction(() => { DatabaseContext.BeginTransaction(); });
    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await TryTransactionAsync((token) => DatabaseContext.BeginTransactionAsync(token), cancellationToken);

    public void CommitTransaction()
        => TryTransaction(() => { DatabaseContext.CommitTransaction(); });

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => await TryTransactionAsync((token) => DatabaseContext.CommitTransactionAsync(token), cancellationToken);
    
    public void RollbackTransaction()
        => TryTransaction(() => { DatabaseContext.RollbackTransaction(); });

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => await TryTransactionAsync((token) => DatabaseContext.RollbackTransactionAsync(token), cancellationToken);

    private void TryTransaction(Action action)
    {
        try
        {
            action();
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LastSaveChangesResult!.Exception = ex;
        }
    }
    
    private async Task TryTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
           await action(cancellationToken);
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LastSaveChangesResult!.Exception = ex;
        }
    }

    #endregion
}