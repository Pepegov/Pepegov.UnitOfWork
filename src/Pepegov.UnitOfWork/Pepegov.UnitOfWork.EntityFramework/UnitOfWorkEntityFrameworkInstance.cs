using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pepegov.UnitOfWork.EntityFramework.Database;
using Pepegov.UnitOfWork.EntityFramework.Repository;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework;

public sealed class UnitOfWorkEntityFrameworkInstance<TContext> : BaseUnitOfWorkInstance, IRepositoryEntityFrameworkInstanceFactory, IUnitOfWorkEntityFrameworkInstance<TContext>
    where TContext : DbContext
{
    private bool _disposed;
    private Dictionary<Type, object>? _repositories;
    
    public TContext DbContext { get; }

    public UnitOfWorkEntityFrameworkInstance(TContext context) : base(new EntityFrameworkDatabaseContext(context))
    {
        DbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region Repository
    
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();
        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new RepositoryEntityFramework<TEntity>(DatabaseContext as IEntityFrameworkDatabaseContext);
        }

        return (IRepositoryEntityFramework<TEntity>)_repositories[type];
    }
    
    public IRepositoryEntityFramework<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();

        static T Cast<T>(object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
        
        if (hasCustomRepository)
        {
            var customRepo = DbContext.GetService<IRepositoryEntityFramework<TEntity>>();
            if (customRepo is not null)
            {
                return customRepo;
            }
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new RepositoryEntityFramework<TEntity>(DatabaseContext as IEntityFrameworkDatabaseContext);
        }

        return (IRepositoryEntityFramework<TEntity>)_repositories[type];
    }
    
    #endregion

    #region SaveChanges

    public void SaveChanges()
    {
        try
        {
            (DatabaseContext as IEntityFrameworkDatabaseContext)?.SaveChanges();
        }
        catch (Exception exception)
        {
            LastSaveChangesResult!.Exception = exception;
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await (DatabaseContext as IEntityFrameworkDatabaseContext)?.SaveChangesAsync(cancellationToken)!;
        }
        catch (Exception exception)
        {
            LastSaveChangesResult!.Exception = exception;
        }
    }
    
    #endregion

    #region Dispose

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _repositories?.Clear();
                DbContext.Dispose();
                DatabaseContext.Dispose();
            }
        }

        _disposed = true;
    }

    #endregion
    
    public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback) =>
        DbContext.ChangeTracker.TrackGraph(rootEntity, callback);

    public void SetAutoDetectChanges(bool value) => DbContext.ChangeTracker.AutoDetectChangesEnabled = value;
}