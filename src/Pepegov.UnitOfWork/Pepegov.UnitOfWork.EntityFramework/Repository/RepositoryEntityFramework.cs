using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.EntityFramework.DatabaseContext;
using Pepegov.UnitOfWork.Extensions;
using Pepegov.UnitOfWork.Search.FuzzySearch;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public class RepositoryEntityFramework<TEntity> : IRepositoryEntityFramework<TEntity> where TEntity : class
{
    private readonly IEntityFrameworkDatabaseContext _databaseContext;
    private readonly DbSet<TEntity> _dbSet;

    public RepositoryEntityFramework(IEntityFrameworkDatabaseContext? databaseContext)
    {
        _databaseContext = databaseContext ?? throw new ArgumentNullException($"{nameof(databaseContext)} is not exist");

        _dbSet = _databaseContext.DbContext.Set<TEntity>();
    }

    public void ChangeEntityState(TEntity entity, EntityState state) => _databaseContext.DbContext.Entry(entity).State = state;

    public void ChangeTable(string table)
    {
        if (_databaseContext.DbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
        {
            relational.SetTableName(table);
        }
    }

    #region GetAll 
    
    public IQueryable<TEntity> GetAll(bool disableTracking = true) =>
        disableTracking
            ? _dbSet.AsNoTracking()
            : _dbSet;

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = true) =>
        disableTracking
            ? _dbSet.AsNoTracking().Select(selector)
            : _dbSet.Select(selector);

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return query.Select(selector);
    }

    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query)
            : query;
    }

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy != null
            ? orderBy(query).Select(selector)
            : query.Select(selector);
    }
    
    public async Task<IList<TEntity>> GetAllWithFuzzySearchAsync(
        string searchQuery,
        Func<TEntity, string> searchProperty,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<List<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false,
        int allowedMistakeDistance = 300)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        var listResult = await query.ToListAsync();
        
        
        var distanceLevenshtein = new DistanceLevenshtein(new char[] { ' ', '-' });
        distanceLevenshtein.SetData(listResult
            .Select(item => new Tuple<string, string>(searchProperty(item), searchProperty(item))).ToList());

        var result = distanceLevenshtein.Search(searchQuery);
        var matches = new List<string>();
        matches.AddRange(result
            .Where(i => i.Item3 <= allowedMistakeDistance)
            .Select(i => i.Item1).ToList());

        listResult = listResult
            .Where(item => matches.
                Any(i => searchProperty(item).ToLower().Contains(i))).ToList();

        return orderBy is null ? listResult : orderBy(listResult).ToList();
    }

    public async Task<IList<TEntity>> GetAllAsync(bool disableTracking = true)
        => disableTracking
            ? await _dbSet.AsNoTracking().ToListAsync()
            : await _dbSet.ToListAsync();

    public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        bool disableTracking = true) =>
        disableTracking
            ? await _dbSet.AsNoTracking().Select(selector).ToListAsync()
            : await _dbSet.Select(selector).ToListAsync();

    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (orderBy is not null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? await orderBy(query).Select(selector).ToListAsync()
            : await query.Select(selector).ToListAsync();
    }

    #endregion

    #region GetPagedList

    public IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query).ToPagedList(pageIndex, pageSize)
            : query.ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
            : query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
    }

    public IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
        where TResult : class
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize)
            : query.Select(selector).ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false)
        where TResult : class
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
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
    
    public async Task<IPagedList<TEntity>> GetPagedListWithFuzzySearchAsync(
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
        int allowedMistakeDistance = 300)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        var listResult = await query.ToListAsync();
        
        
        var distanceLevenshtein = new DistanceLevenshtein(new char[] { ' ', '-' });
        distanceLevenshtein.SetData(listResult
            .Select(item => new Tuple<string, string>(searchProperty(item), searchProperty(item))).ToList());

        var result = distanceLevenshtein.Search(searchQuery);
        var matches = new List<string>();
        matches.AddRange(result
            .Where(i => i.Item3 <= allowedMistakeDistance)
            .Select(i => i.Item1).ToList());

        listResult = listResult
            .Where(item => matches.
                Any(i => searchProperty(item).ToLower().Contains(i))).ToList();
        
        return orderBy is not null
            ? await orderBy(listResult).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
            : await query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
    }

    #endregion

    #region GetFirstOrDefault

    public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query).FirstOrDefault()
            : query.FirstOrDefault();
    }


    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? await orderBy(query).FirstOrDefaultAsync()
            : await query.FirstOrDefaultAsync();
    }

    public TResult? GetFirstOrDefault<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).FirstOrDefault()
            : query.Select(selector).FirstOrDefault();
    }

    public async Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false)
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

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return orderBy is not null
            ? await orderBy(query).Select(selector).FirstOrDefaultAsync()
            : await query.Select(selector).FirstOrDefaultAsync();
    }

    #endregion

    #region Find

    public TEntity? Find(params object[] keyValues) => _dbSet.Find(keyValues);

    public ValueTask<TEntity?> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

    public ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken) =>
        _dbSet.FindAsync(keyValues, cancellationToken);

    #endregion

    #region Count

    public int Count(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Count()
            : _dbSet.Count(predicate);

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);

    public long LongCount(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.LongCount()
            : _dbSet.LongCount(predicate);

    public async Task<long> LongCountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? await _dbSet.LongCountAsync(cancellationToken)
            : await _dbSet.LongCountAsync(predicate, cancellationToken);

    #endregion

    #region Exist

    public bool Exists(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Any()
            : _dbSet.Any(predicate);

    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>>? selector = null,
        CancellationToken cancellationToken = default) =>
        selector is null
            ? await _dbSet.AnyAsync(cancellationToken)
            : await _dbSet.AnyAsync(selector, cancellationToken);

    #endregion

    #region Max

    public T? Max<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Max(selector)
            : _dbSet.Where(predicate).Max(selector);

    public Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.MaxAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).MaxAsync(selector, cancellationToken);

    #endregion

    #region Min
    
    public T? Min<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Min(selector)
            : _dbSet.Where(predicate).Min(selector);

    public Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.MinAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).MinAsync(selector, cancellationToken);

    #endregion

    #region Average

    public decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Average(selector)
            : _dbSet.Where(predicate).Average(selector);

    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.AverageAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);

    #endregion

    #region Sum

    public decimal Sum(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Sum(selector)
            : _dbSet.Where(predicate).Sum(selector);

    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.SumAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).SumAsync(selector, cancellationToken);

    #endregion

    #region Insert

    public TEntity Insert(TEntity entity) => _dbSet.Add(entity).Entity;

    public void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

    public void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

    public ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken);

    public Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

    public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        _dbSet.AddRangeAsync(entities, cancellationToken);
    
    #endregion

    #region Update

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void UpdateAsync(TEntity entity) => _dbSet.Update(entity);

    public void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

    public void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    #endregion

    #region Delete

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void Delete(object id)
    {
        var typeInfo = typeof(TEntity).GetTypeInfo();
        var key = _databaseContext.DbContext.Model.FindEntityType(typeInfo)?.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (key is null)
        {
            return;
        }

        var property = typeInfo.GetProperty(key.Name);
        if (property != null)
        {
            var entity = Activator.CreateInstance<TEntity>();
            property.SetValue(entity, id);
            _databaseContext.DbContext.Entry(entity).State = EntityState.Deleted;
        }
        else
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }
    }

    public void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

    public void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    
    #endregion
}

public static class PagedList
{
    public static IPagedList<T> Empty<T>() => new PagedList<T>();

    public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source,
        Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) =>
        new PagedList<TSource, TResult>(source, converter);
}