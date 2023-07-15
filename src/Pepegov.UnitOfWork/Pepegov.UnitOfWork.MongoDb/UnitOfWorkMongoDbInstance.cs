using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Repository;

namespace Pepegov.UnitOfWork.MongoDb;

/// <summary>
/// Represents the default implementation of the <see cref="T:IUnitOfWorkInstance"/>
/// and <see cref="T:IUnitOfWorkMongoDbInstance{TContext}"/> interface.
/// </summary>
public sealed class UnitOfWorkMongoDbInstance : BaseUnitOfWorkInstance, IUnitOfWorkMongoInstance
{
    private readonly ILogger<IUnitOfWorkMongoInstance> _logger;
    private readonly ICollectionNameSelector _collectionNameSelector;
    private readonly bool _canHaveMongoDbTransactions;
    private bool _disposed;
    private Dictionary<Type, object>? _repositories;

    public UnitOfWorkMongoDbInstance(
        ILogger<IUnitOfWorkMongoInstance> logger,
        ILogger<IMongoDatabaseContext> loggerContext,
        ICollectionNameSelector collectionNameSelector,
        IMongoDatabaseContext databaseContext) : base(databaseContext)
    {
        _logger = logger;
        _collectionNameSelector = collectionNameSelector;
    }

    #region GetRepository

    /// <summary>
    /// Returns repository wrapper for MongoDb collection
    /// </summary>
    /// <typeparam name="TDocument">type of Document</typeparam>
    /// <param name="writeConcern">default </param>
    /// <param name="readConcern"></param>
    /// <param name="readPreference"></param>
    public IRepositoryMongo<TDocument> GetRepository<TDocument>(
        WriteConcern? writeConcern = null,
        ReadConcern? readConcern = null,
        ReadPreference? readPreference = null) where TDocument : class
    {
        _repositories ??= new Dictionary<Type, object>();
        var type = typeof(TDocument);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new RepositoryMongo<TDocument>(
                (DatabaseContext as IMongoDatabaseContext)!, 
                _logger, _collectionNameSelector, writeConcern, readConcern, readPreference);
        }

        return (IRepositoryMongo<TDocument>)_repositories[type];
    }

    #endregion
    
    #region GetSession

    /// <summary>
    /// Returns session from current client collection <see cref="IMongoClient"/>
    /// </summary>
    public IClientSessionHandle? GetSession()
    {
        return (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client!.StartSession();
    }

    /// <summary>
    /// Returns session from current client collection <see cref="IMongoClient"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    public Task<IClientSessionHandle>? GetSessionAsync(CancellationToken cancellationToken)
    {
        return (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client!.StartSessionAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region UseTransaction

    /// <summary>
    /// Runs awaitable method in transaction scope. With new instance of repository creation.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <param name="session">session</param>
    /// <param name="transactionOptions">options</param>
    public async Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, IClientSessionHandle, CancellationToken, Task> taskOperation,
        CancellationToken cancellationToken,
        IClientSessionHandle? session,
        TransactionOptions? transactionOptions = null) where TDocument : class
    {
        using var session1 = session ?? await (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client.StartSessionAsync(null, cancellationToken)!;

        try
        {
            var repository = GetRepository<TDocument>();
            var options = transactionOptions ?? new TransactionOptions(
                readPreference: ReadPreference.Primary,
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority);

            session1.StartTransaction(options);

            await taskOperation(repository, session1, cancellationToken);

            await session1.CommitTransactionAsync(cancellationToken);

        }
        catch (NotSupportedException)
        {
            throw;
        }

        catch (Exception exception)
        {
            await session1.AbortTransactionAsync(cancellationToken);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogError("[TRANSACTION ROLLBACK] {Id} {Message}", session1.ServerSession.Id, exception.Message);
            }

            throw;
        }
    }

    /// <summary>
    /// Runs awaitable method in transaction scope. With new instance of repository creation.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="transactionContext">transaction context object</param>
    public async Task UseTransactionAsync<TDocument>(
        Func<IRepositoryMongo<TDocument>, TransactionContext, Task> taskOperation,
        TransactionContext transactionContext)
        where TDocument : class
    {
        transactionContext.SetLogger(_logger);

        var repository = GetRepository<TDocument>();

        using var session = transactionContext.Session ?? await (DatabaseContext as IMongoDatabaseContext)!.DatabaseBuilder.Build().Client.StartSessionAsync(null, transactionContext.CancellationToken);

        try
        {
            var options = transactionContext.TransactionOptions ??
                          new TransactionOptions(
                              readPreference: ReadPreference.Primary,
                              readConcern: ReadConcern.Snapshot,
                              writeConcern: WriteConcern.WMajority);

            session.StartTransaction(options);

            await taskOperation(repository, transactionContext);

            await session.CommitTransactionAsync(transactionContext.CancellationToken);

        }
        catch (NotSupportedException)
        {
            throw;
        }

        catch (Exception exception)
        {
            await session.AbortTransactionAsync(transactionContext.CancellationToken);
            if (transactionContext.Logger.IsEnabled(LogLevel.Information))
            {
                transactionContext.Logger.LogError("[TRANSACTION ROLLBACK] {Id} {Message}", session.ServerSession.Id, exception.Message);
            }

            throw;
        }
    }

    /// <summary>
    /// Runs awaitable method in transaction scope. Using instance of the repository already exist.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="repository">instance of the repository</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <param name="session">session</param>
    /// <param name="transactionOptions">options</param>
    public async Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, IClientSessionHandle, CancellationToken, Task> taskOperation,
        IRepositoryMongo<TDocument> repository,
        CancellationToken cancellationToken,
        IClientSessionHandle? session,
        TransactionOptions? transactionOptions = null) where TDocument : class
    {
        using var session1 = session ?? await (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client.StartSessionAsync(null, cancellationToken)!;

        try
        {
            var options = transactionOptions ?? new TransactionOptions(
                readPreference: ReadPreference.Primary,
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority);

            session1.StartTransaction(options);

            await taskOperation(repository, session1, cancellationToken);

            await session1.CommitTransactionAsync(cancellationToken);

        }
        catch (NotSupportedException)
        {
            throw;
        }

        catch (Exception exception)
        {
            await session1.AbortTransactionAsync(cancellationToken);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogError("[TRANSACTION ROLLBACK] {Id} {Message}", session1.ServerSession.Id, exception.Message);
            }

            throw;
        }
    }

    /// <summary>
    /// Runs awaitable method in transaction scope. Using instance of the repository already exist.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="repository">instance of the repository</param>
    /// <param name="transactionContext">Transaction context with additional helpful instances for operation</param>
    /// <returns></returns>
    public async Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, TransactionContext, Task> taskOperation,
        IRepositoryMongo<TDocument> repository,
        TransactionContext transactionContext)
        where TDocument : class
    {
        transactionContext.SetLogger(_logger);

        using var session = transactionContext.Session ?? await (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client.StartSessionAsync(null, transactionContext.CancellationToken)!;

        try
        {
            var options = transactionContext.TransactionOptions ?? new TransactionOptions(
                readPreference: ReadPreference.Primary,
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority);

            session.StartTransaction(options);

            await taskOperation(repository, transactionContext);

            await session.CommitTransactionAsync(transactionContext.CancellationToken);

        }
        catch (NotSupportedException)
        {
            throw;
        }

        catch (Exception exception)
        {
            await session.AbortTransactionAsync(transactionContext.CancellationToken);
            if (transactionContext.Logger.IsEnabled(LogLevel.Information))
            {
                transactionContext.Logger.LogError("[TRANSACTION ROLLBACK] {Id} {Message}", session.ServerSession.Id, exception.Message);
            }

            throw;
        }
    }
    
    #endregion

    #region Disposable

    public override void Dispose()
    {
        Dispose(true);

        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _repositories?.Clear();
            }
        }
        _disposed = true;
    }

    #endregion

    /// <summary>
    /// Tests that a transaction available in MongoDb replica set
    /// </summary>
    public bool EnsureReplicationSetReady()
    {
        var result=  (DatabaseContext as IMongoDatabaseContext)?.MongoDatabase.Client.EnsureReplicationSetReady();
        ArgumentNullException.ThrowIfNull(result);
        return (bool)result;
    }
}