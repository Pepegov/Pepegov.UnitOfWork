using System.Data;

namespace Pepegov.UnitOfWork.AdoNet.DatabaseContext;

public class AdoNetDatabaseContext : IDatabaseContext, IDisposable
{
    private IDbConnection _connection;
    private ITransactionAdapter? _transaction;
    public ITransactionAdapter Transaction { get; } //fix

    public AdoNetDatabaseContext(IDbConnection dbConnection)
    {
        _connection = dbConnection;
        _connection.Open();
    }
    
    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        _connection.Close();
        _connection.Dispose();
    }

    public IDbCommand CreateCommand()
    {
        var command = _connection.CreateCommand();
        //command.Transaction = _transaction;
        return command;
    }

    public void SaveChanges()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Transaction have already been already been commited. Check your transaction handling.");
        }
        _transaction.Commit();
        _transaction = null;
    }

    public Task SaveChangesAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Transaction have already been already been commited. Check your transaction handling.");
        }
        _transaction.Commit();
        _transaction = null;
        return Task.CompletedTask;
    }

    public void BeginTransaction()
    {
        //_transaction = _connection.BeginTransaction();
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        //_transaction = _connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction?.Commit();
        return Task.CompletedTask;
    }

    public void RollbackTransaction()
    {
        _transaction?.Rollback();
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction?.Rollback();
        return Task.CompletedTask;
    }
}