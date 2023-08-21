using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pepegov.UnitOfWork.EntityFramework.Database;

public class EntityFrameworkTransaction : ITransactionAdapter
{
    private IDbContextTransaction? _efDbTransaction;
    public DbContext DbContext { get; private set; }

    public EntityFrameworkTransaction(DbContext context)
    {
        DbContext = context;
    }
    
    public void BeginTransaction()
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            transaction = DbContext.Database.BeginTransaction();
        }

        _efDbTransaction = transaction;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        _efDbTransaction = transaction;
    }

    public void Commit()
    {
        _efDbTransaction?.Commit();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _efDbTransaction?.CommitAsync(cancellationToken)!;
    }

    public void Rollback()
    {
        _efDbTransaction?.Rollback();
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return _efDbTransaction?.RollbackAsync(cancellationToken)!;
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}