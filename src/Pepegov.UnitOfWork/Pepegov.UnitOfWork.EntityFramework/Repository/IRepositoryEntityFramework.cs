using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public interface IRepositoryEntityFramework<TEntity> : IRepository<TEntity>, IRepositoryQueryableExpression<TEntity>, IRepositoryQueryableEntityFramework<TEntity> where TEntity : class
{
    #region Paged

    IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false);

    IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false) where TResult : class;

    Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false) where TResult : class;
    
    Task<IPagedList<TEntity>> GetPagedListWithFuzzySearchAsync(
        string searchQuery,
        Func<TEntity, string> searchProperty,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<List<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false,
        int allowedMistakeDistance = 300);

    #endregion

    #region FirstOrDefault

    TEntity? GetFirstOrDefault(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    TResult? GetFirstOrDefault<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Find

    TEntity? Find(params object[] keyValues);

    ValueTask<TEntity?> FindAsync(params object[] keyValues);

    ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken);

    #endregion

    #region GetAll

    IQueryable<TEntity> GetAll(bool disableTracking = true, bool ignoreQueryFilters = false);

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    Task<IList<TEntity>> GetAllAsync(
        bool disableTracking = true, 
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = true, 
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);

    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);


    Task<IList<TEntity>> GetAllWithFuzzySearchAsync(
        string searchQuery,
        Func<TEntity, string> searchProperty,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<List<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false,
        int allowedMistakeDistance = 300);
    

    #endregion

    #region Insert

    TEntity Insert(TEntity entity);

    void Insert(params TEntity[] entities);

    void Insert(IEnumerable<TEntity> entities);

    ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity,
        CancellationToken cancellationToken = default(CancellationToken));

    Task InsertAsync(params TEntity[] entities);

    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

    #endregion

    #region Update

    void Update(TEntity entity);

    void Update(params TEntity[] entities);

    void Update(IEnumerable<TEntity> entities);

    #endregion

    #region Delete

    void Delete(object id);

    void Delete(TEntity entity);

    void Delete(params TEntity[] entities);

    void Delete(IEnumerable<TEntity> entities);

    #endregion

    #region Count

    int Count(Expression<Func<TEntity, bool>>? predicate = null, 
        bool ignoreQueryFilters = false);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, 
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    long LongCount(Expression<Func<TEntity, bool>>? predicate = null, 
        bool ignoreQueryFilters = false);

    Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Exist

    bool Exists(Expression<Func<TEntity, bool>>? predicate = null, bool ignoreQueryFilters = false);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion
    
    #region Max

    public T? Max<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false);

    public Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Min

    public T? Min<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false);
    
    public Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Average

    public decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false);
    
    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);
    
    #endregion

    #region Sum

    public decimal Sum(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false);
    
    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    #endregion

    void ChangeEntityState(TEntity entity, EntityState state);
}