using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public partial class RepositoryEntityFramework<TEntity> : IRepositoryQueryableEntityFramework<TEntity> where TEntity : class
{
    public TResult Query<TResult>(Func<IQueryable, TResult> func)
    {
        IQueryable<TEntity> query = _dbSet;
        return func.Invoke(query);
    }

    public Task<TResult> QueryAsync<TResult>(Func<IQueryable, Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        return func.Invoke(query);
    }

    public TResult Query<TResult>(
        Func<IQueryable, TResult> func, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return func.Invoke(query);
    }

    public Task<TResult> QueryAsync<TResult>(
        Func<IQueryable, Task<TResult>> func, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return func.Invoke(query);
    }

    public TResult Query<TResult>(
        Func<IQueryable, TResult> func, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (include is not null)
        {
            query = include(query);
        }
        
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return func.Invoke(query);
    }

    public Task<TResult> QueryAsync<TResult>(
        Func<IQueryable, Task<TResult>> func, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (include is not null)
        {
            query = include(query);
        }
        
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return func.Invoke(query);
    }
    
    
    public IQueryable<TEntity> Query(Action<IQueryable> func)
    {
        IQueryable<TEntity> query = _dbSet;
        func.Invoke(query);
        return query;
    }

    public IQueryable<TEntity> Query(
        Action<IQueryable> func, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        func.Invoke(query);
        
        return query;
    }
    
    public IQueryable<TEntity> Query(
        Action<IQueryable> func, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        bool disableTracking = true, 
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (include is not null)
        {
            query = include(query);
        }
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        func.Invoke(query);
        
        return query;
    }
}