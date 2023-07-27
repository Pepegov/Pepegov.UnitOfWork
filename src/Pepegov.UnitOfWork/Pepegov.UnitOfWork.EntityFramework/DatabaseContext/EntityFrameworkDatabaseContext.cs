using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pepegov.UnitOfWork.EntityFramework.DatabaseContext;

public class EntityFrameworkDatabaseContext: IEntityFrameworkDatabaseContext  
{
    public DbContext DbContext { get; private set; }
    public IDbTransaction Transaction { get; private set; }
    private IDbContextTransaction? _efDbTransaction;
    
    public EntityFrameworkDatabaseContext(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void SaveChanges()
    {
        DbContext.SaveChanges();
        //_efDbTransaction?.Commit();
        //Transaction?.Commit();
    }

    public async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
        //await _efDbTransaction?.CommitAsync()!;
    }

    public void BeginTransaction()
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            transaction = DbContext.Database.BeginTransaction();
        }

        _efDbTransaction = transaction;
        Transaction = transaction.GetDbTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        _efDbTransaction = transaction;
        Transaction = transaction.GetDbTransaction();
    }

    public void CommitTransaction()
    {
        _efDbTransaction?.Commit();
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _efDbTransaction?.CommitAsync(cancellationToken)!;
    }

    public void RollbackTransaction()
    {
        _efDbTransaction?.Rollback();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _efDbTransaction?.RollbackAsync(cancellationToken)!;
    }

    public void Dispose()
    {
        //TODO: fix that
        //Transaction.Rollback();
        //Transaction.Dispose();
        //_efDbTransaction?.Rollback();
        //_efDbTransaction?.Dispose();
        //DbContext.Dispose();
    }
}