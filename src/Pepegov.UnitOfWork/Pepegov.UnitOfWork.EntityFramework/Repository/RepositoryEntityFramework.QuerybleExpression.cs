using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Extensions;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public partial class RepositoryEntityFramework<TEntity> where TEntity : class
{
    #region Paged

    public IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        int pageIndex = 0, int pageSize = 20)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return orderBy is not null
            ? orderBy(query).ToPagedList(pageIndex, pageSize)
            : query.ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        int pageIndex = 0, int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
            : query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
    }

    public IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<TEntity, TResult>> selector, 
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageIndex = 0, int pageSize = 20) where TResult : class
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize)
            : query.Select(selector).ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageIndex = 0, int pageSize = 20, CancellationToken cancellationToken = default) where TResult : class
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            return orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }
        else
        {
            return query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }
    }

    #endregion

    #region FirstOrDefaultAsync

    public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).FirstOrDefault()
            : query.FirstOrDefault();
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? await orderBy(query).FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region GetAll

    public IQueryable<TEntity> GetAll()
    {
        IQueryable<TEntity> query = _dbSet;
        return query;
    }

    public IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return query.Select(selector);
    }

    public IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return query.Select(selector);
    }

    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query)
            : query;
    }

    public IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null
            ? orderBy(query).Select(selector)
            : query.Select(selector);
    }

    public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        return await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            return await orderBy(query).ToListAsync(cancellationToken: cancellationToken);
        }

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? await orderBy(query).Select(selector).ToListAsync(cancellationToken: cancellationToken)
            : await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region Count

    public int Count(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;

        return predicate is null
            ? query.Count()
            : query.Count(predicate);;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? await query.CountAsync(cancellationToken)
            : await query.CountAsync(predicate, cancellationToken);
    }
    
    public long LongCount(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.LongCount()
            : query.LongCount(predicate);
    }

    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        return predicate is null
            ? await query.LongCountAsync(cancellationToken)
            : await query.LongCountAsync(predicate, cancellationToken);
    }

    #endregion

    #region Exists

    public bool Exists(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.Any()
            : query.Any(predicate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return selector is null
            ? await query.AnyAsync(cancellationToken)
            : await query.AnyAsync(selector, cancellationToken);
    }

    #endregion

    #region Max

    public T? Max<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;

        return predicate is null
            ? query.Max(selector)
            : query.Where(predicate).Max(selector);
    }

    public Task<T> MaxAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        return predicate is null
            ? query.MaxAsync(selector, cancellationToken)
            : query.Where(predicate).MaxAsync(selector, cancellationToken);
    }

    #endregion

    #region Min

    public T? Min<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.Min(selector)
            : query.Where(predicate).Min(selector);
    }

    public Task<T> MinAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.MinAsync(selector, cancellationToken)
            : query.Where(predicate).MinAsync(selector, cancellationToken);
    }

    #endregion

    #region Average

    public decimal Average(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.Average(selector)
            : query.Where(predicate).Average(selector);
    }   
    
    public Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.AverageAsync(selector, cancellationToken)
            : query.Where(predicate).AverageAsync(selector, cancellationToken);
    }

    #endregion

    #region Sum

    public decimal Sum(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.Sum(selector)
            : query.Where(predicate).Sum(selector);
    }

    public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        return predicate is null
            ? query.SumAsync(selector, cancellationToken)
            : query.Where(predicate).SumAsync(selector, cancellationToken);
    }

    #endregion
}