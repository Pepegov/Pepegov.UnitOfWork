using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork.Repository;

public interface IRepositoryLinq<TEntity> : IRepository<TEntity> where TEntity : class
{
    #region GetFirstOrDefault
    
    TEntity? GetFirstOrDefault(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
    
    #endregion
    
    #region GetAll

    IQueryable<TEntity> GetAll();

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector);

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);

    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    Task<IList<TEntity>> GetAllAsync();

    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector);

    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);


    Task<IList<TEntity>> GetAllWithFuzzySearchAsync(
        string searchQuery,
        Func<TEntity, string> searchProperty,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int allowedMistakeDistance = 300);
    

    #endregion

    #region Count

    int Count(Expression<Func<TEntity, bool>>? predicate = null);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    long LongCount(Expression<Func<TEntity, bool>>? predicate = null);

    Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Exist

    bool Exists(Expression<Func<TEntity, bool>>? predicate = null);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null,
        CancellationToken cancellationToken = default);

    #endregion
    
    #region Max

    public T? Max<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);

    public Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Min

    public T? Min<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);
    
    public Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Average

    public decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);
    
    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    #endregion

    #region Sum

    public decimal Sum(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);
    
    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion
}