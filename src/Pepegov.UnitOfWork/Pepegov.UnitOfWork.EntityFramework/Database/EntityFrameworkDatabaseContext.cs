using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pepegov.UnitOfWork.EntityFramework.Database;

public class EntityFrameworkDatabaseContext : IEntityFrameworkDatabaseContext  
{
    public DbContext DbContext { get; private set; }
    public ITransactionAdapter? Transaction { get; private set; }

    public EntityFrameworkDatabaseContext(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void BeginTransaction()
    {
        Transaction ??= new EntityFrameworkTransaction(DbContext);
        Transaction.BeginTransaction(); 
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Transaction ??= new EntityFrameworkTransaction(DbContext);
        return Transaction.BeginTransactionAsync(cancellationToken);
    }

    public void CommitTransaction()
    {
        ArgumentNullException.ThrowIfNull(Transaction, "Transaction != null");
        Transaction.Commit();
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(Transaction, "Transaction != null");
        return Transaction.CommitAsync(cancellationToken);
    }

    public void RollbackTransaction()
    {
        ArgumentNullException.ThrowIfNull(Transaction, "Transaction != null");
        Transaction.RollbackAsync();
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(Transaction, "Transaction != null");
        return Transaction.RollbackAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        DbContext.SaveChanges();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
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