using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Repository;

namespace Pepegov.UnitOfWork.MongoDb;

/// <summary>
/// Unit of Work interface for pattern implementation
/// </summary>
public interface IUnitOfWorkMongoInstance : IUnitOfWorkInstance, IRepositoryMongoFactory
{
    #region GetSession

    /// <summary>
    /// Returns session from current client collection <see cref="IMongoClient"/>
    /// </summary>
    IClientSessionHandle? GetSession();

    /// <summary>
    /// Returns session from current client collection <see cref="IMongoClient"/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IClientSessionHandle>? GetSessionAsync(CancellationToken cancellationToken);

    #endregion
    
    #region Transaction 
    
    /// <summary>
    /// Runs awaitable method in transaction scope. With new instance of repository creation.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <param name="session">session</param>
    /// <param name="transactionOptions">options</param>
    Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, IClientSessionHandle, CancellationToken, Task> taskOperation,
        CancellationToken cancellationToken,
        IClientSessionHandle? session,
        TransactionOptions? transactionOptions = null)
        where TDocument : class;

    /// <summary>
    /// Runs awaitable method in transaction scope. With new instance of repository creation.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="transactionContext">transaction context object</param>
    Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, TransactionContext, Task> taskOperation,
        TransactionContext transactionContext)
        where TDocument : class;

    /// <summary>
    /// Runs awaitable method in transaction scope. Using instance of the repository already exist.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="repository">instance of the repository</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <param name="session">session</param>
    /// <param name="transactionOptions">options</param>
    Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, IClientSessionHandle, CancellationToken, Task> taskOperation,
        IRepositoryMongo<TDocument> repository,
        CancellationToken cancellationToken,
        IClientSessionHandle? session,
        TransactionOptions? transactionOptions = null) where TDocument : class;

    /// <summary>
    /// Runs awaitable method in transaction scope. Using instance of the repository already exist.
    /// </summary>
    /// <typeparam name="TDocument">type of the repository entity</typeparam>
    /// <param name="taskOperation">operation will run in transaction</param>
    /// <param name="repository">instance of the repository</param>
    /// <param name="transactionContext">Transaction context with additional helpful instances for operation</param>
    /// <returns></returns>
    Task UseTransactionAsync<TDocument>
    (
        Func<IRepositoryMongo<TDocument>, TransactionContext, Task> taskOperation,
        IRepositoryMongo<TDocument> repository,
        TransactionContext transactionContext)
        where TDocument : class;
    
    #endregion
    
    /// <summary>
    /// Ensures that the MongoDb replica set enabled.
    /// Warning: Do not use this method on the production.
    /// </summary>
    bool EnsureReplicationSetReady();

}

