using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.EntityFramework.Database;
using Pepegov.UnitOfWork.Extensions;
using Pepegov.UnitOfWork.Search.FuzzySearch;

namespace Pepegov.UnitOfWork.EntityFramework.Repository;

public partial class RepositoryEntityFramework<TEntity> : IRepositoryEntityFramework<TEntity> where TEntity : class
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

    public IQueryable<TEntity> GetAll(bool disableTracking = true, bool ignoreQueryFilters = false)
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

        return query;
    }

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
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
        
        return query.Select(selector);
    }
        

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
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
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
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

    public async Task<IList<TEntity>> GetAllAsync(bool disableTracking = true, bool ignoreQueryFilters = false,
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

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
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

        return await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
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
            return await orderBy(query).ToListAsync(cancellationToken: cancellationToken);
        }

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
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
            ? await orderBy(query).Select(selector).ToListAsync(cancellationToken: cancellationToken)
            : await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
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
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
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
            ? await orderBy(query).FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
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
        bool disableTracking = true, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
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
            ? await orderBy(query).Select(selector).FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await query.Select(selector).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region Find

    public TEntity? Find(params object[] keyValues) => _dbSet.Find(keyValues);

    public ValueTask<TEntity?> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

    public ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken) =>
        _dbSet.FindAsync(keyValues, cancellationToken);

    #endregion

    #region Count

    public int Count(Expression<Func<TEntity, bool>>? predicate = null, bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return predicate is null
            ? query.Count()
            : query.Count(predicate);;
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return predicate is null
            ? await query.CountAsync(cancellationToken)
            : await query.CountAsync(predicate, cancellationToken);
    }

    public long LongCount(Expression<Func<TEntity, bool>>? predicate = null, bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return predicate is null
            ? query.LongCount()
            : query.LongCount(predicate);
    }

    public async Task<long> LongCountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return predicate is null
            ? await query.LongCountAsync(cancellationToken)
            : await query.LongCountAsync(predicate, cancellationToken);
    }

    #endregion

    #region Exist

    public bool Exists(Expression<Func<TEntity, bool>>? predicate = null, bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return predicate is null
            ? query.Any()
            : query.Any(predicate);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>>? selector = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return selector is null
            ? await query.AnyAsync(cancellationToken)
            : await query.AnyAsync(selector, cancellationToken);
    }

    #endregion

    #region Max

    public T? Max<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return predicate is null
            ? query.Max(selector)
            : query.Where(predicate).Max(selector);
    }

    public Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        
        return predicate is null
            ? query.MaxAsync(selector, cancellationToken)
            : query.Where(predicate).MaxAsync(selector, cancellationToken);
    }

    #endregion

    #region Min

    public T? Min<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.Min(selector)
            : query.Where(predicate).Min(selector);
    }

    public Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.MinAsync(selector, cancellationToken)
            : query.Where(predicate).MinAsync(selector, cancellationToken);
    }

    #endregion

    #region Average

    public decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.Average(selector)
            : query.Where(predicate).Average(selector);
    }

    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.AverageAsync(selector, cancellationToken)
            : query.Where(predicate).AverageAsync(selector, cancellationToken);
    }

    #endregion

    #region Sum

    public decimal Sum(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.Sum(selector)
            : query.Where(predicate).Sum(selector);
    }

    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }   
        
        return predicate is null
            ? query.SumAsync(selector, cancellationToken)
            : query.Where(predicate).SumAsync(selector, cancellationToken);
    }

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