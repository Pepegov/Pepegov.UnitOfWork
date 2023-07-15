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
    
    public async Task BeginTransactionAsync()
        => await TryTransactionAsync(() => DatabaseContext.BeginTransactionAsync());

    public void CommitTransaction()
        => TryTransaction(() => { DatabaseContext.CommitTransaction(); });

    public async Task CommitTransactionAsync()
        => await TryTransactionAsync(() => DatabaseContext.CommitTransactionAsync());
    
    public void RollbackTransaction()
        => TryTransaction(() => { DatabaseContext.RollbackTransaction(); });

    public async Task RollbackTransactionAsync()
        => await TryTransactionAsync(() => DatabaseContext.RollbackTransactionAsync());

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
    
    private async Task TryTransactionAsync(Func<Task> action)
    {
        try
        {
           await action();
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