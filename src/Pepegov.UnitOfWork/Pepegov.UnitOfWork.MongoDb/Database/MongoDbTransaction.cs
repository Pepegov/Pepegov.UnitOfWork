using System.Data;
using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Database;

public class MongoDbTransaction : ITransactionAdapter
{
    private readonly IClientSessionHandle _session;
    private readonly TransactionOptions _transactionOptions;

    public MongoDbTransaction(TransactionContext transactionContext)
    {
        _transactionOptions= transactionContext.TransactionOptions ?? new TransactionOptions(
            readPreference: ReadPreference.Primary,
            readConcern: ReadConcern.Snapshot,
            writeConcern: WriteConcern.WMajority);
        _session = transactionContext.Session;
    }

    public void BeginTransaction()
    {
        _session.StartTransaction(_transactionOptions);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _session.StartTransaction();
        return Task.CompletedTask;
    }
    
    public void Commit()
    {
        _session.CommitTransaction();
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _session.CommitTransactionAsync(cancellationToken);
    }

    public void Rollback()
    {
        _session.AbortTransaction();
    }
    
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _session.AbortTransactionAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        if (_session.IsInTransaction)
        {
            _session.AbortTransaction();
        }
        _session.Dispose();
        GC.SuppressFinalize(this);   
    }

    public IDbConnection? Connection { get; }
    public IsolationLevel IsolationLevel { get; }
}