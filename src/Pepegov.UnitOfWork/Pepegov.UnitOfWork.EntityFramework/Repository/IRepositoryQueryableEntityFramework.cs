using Microsoft.EntityFrameworkCore.Query;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public interface IRepositoryQueryableEntityFramework<TEntity>: IRepositoryQueryable<TEntity> where TEntity : class
{
    public TResult Query<TResult>(
        Func<IQueryable<TEntity>, TResult> func,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    public Task<TResult> QueryAsync<TResult>(
        Func<IQueryable<TEntity>, Task<TResult>> func,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);
    
    public TResult Query<TResult>(
        Func<IQueryable<TEntity>, TResult> func,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    public Task<TResult> QueryAsync<TResult>(
        Func<IQueryable<TEntity>, Task<TResult>> func,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);
    
    public IQueryable<TEntity> Query(
        Action<IQueryable<TEntity>> func,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);
    
    public IQueryable<TEntity> Query(
        Action<IQueryable<TEntity>> func,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);
}