using System;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Pepegov.UnitOfWork.MongoDb.Database;

public class MongoDatabaseContext : IMongoDatabaseContext
{
    private bool _disposed;
    
    private readonly ILogger<IMongoDatabaseContext> _logger;

    //TODO: add async transaction version
    public ITransactionAdapter? Transaction { get; private set; }
    public IMongoDatabase MongoDatabase { get; private set; }
    public IDatabaseBuilder DatabaseBuilder { get; private set; }
    public IClientSessionHandle SessionHandle { get; private set; }

    public MongoDatabaseContext(ILogger<IMongoDatabaseContext> logger, IDatabaseBuilder databaseBuilder)
    {
        _logger = logger;
        DatabaseBuilder = databaseBuilder;

        MongoDatabase = databaseBuilder.Build();
        SessionHandle = MongoDatabase.Client.StartSession();
    }
    
    #region Transaction

    public void BeginTransaction()
    {
        Transaction ??= new MongoDbTransaction(
            new TransactionContext(null, SessionHandle));
        Transaction.BeginTransaction(); 
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Transaction ??= new MongoDbTransaction(
            new TransactionContext(null, SessionHandle, cancellationToken));
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
        Transaction.Rollback();
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(Transaction, "Transaction != null");
        return Transaction.RollbackAsync(cancellationToken);
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        if (!_disposed)
        {
            if (Transaction is not null)
            {
                Transaction.Dispose();
                SessionHandle = null;
            }
            else
            {
                SessionHandle.Dispose();
            }
            MongoDatabase = null;
            DatabaseBuilder = null;
        }

        _disposed = true;
    }

    #endregion
}