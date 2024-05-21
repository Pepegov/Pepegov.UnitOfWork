using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Extensions;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.MongoDb.Repository;

public partial class RepositoryMongo<TDocument>  where TDocument : class
{
    //TODO test this region
    #region Find

    public TDocument? Find(params object[] keyValues)
    {
        var item =  Collection.Find(Builders<TDocument>.Filter.Eq(IdInDbName, keyValues));
        return item.ToList().FirstOrDefault();
    }

    public async ValueTask<TDocument?> FindAsync(params object[] keyValues)
    {
        var item = await Collection.FindAsync(Builders<TDocument>.Filter.Eq(IdInDbName, keyValues));
        return await item.FirstOrDefaultAsync();
    }

    public async ValueTask<TDocument?> FindAsync(object[] keyValues, CancellationToken cancellationToken)
    {
        var item = await Collection.FindAsync(Builders<TDocument>.Filter.Eq(IdInDbName, keyValues), cancellationToken: cancellationToken);
        return await item.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    #endregion
    
    #region Insert

    public void Insert(params TDocument[] entities)
    {
        Collection.InsertMany(_databaseContext.SessionHandle, entities);
    }

    public void Insert(IEnumerable<TDocument> entities)
    {
        Collection.InsertMany(_databaseContext.SessionHandle, entities);
    }

    public Task InsertAsync(params TDocument[] entities)
    {
        return Collection.InsertManyAsync(_databaseContext.SessionHandle, entities);
    }

    public Task InsertAsync(IEnumerable<TDocument> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        return Collection.InsertManyAsync(_databaseContext.SessionHandle, entities, cancellationToken: cancellationToken);
    }

    #endregion

    #region Update

    public void Update(TDocument entity)
    {
        Collection.FindOneAndUpdate(_databaseContext.SessionHandle,
            filter: Builders<TDocument>.Filter.Eq(IdInDbName, GetIdValue(entity)), new ObjectUpdateDefinition<TDocument>(entity));
    }

    public void Update(params TDocument[] entities)
    {
        foreach (var entity in entities)
        {
            Collection.FindOneAndUpdate(_databaseContext.SessionHandle,
                filter: Builders<TDocument>.Filter.Eq(IdInDbName, GetIdValue(entity)), new ObjectUpdateDefinition<TDocument>(entity));
        }
    }

    public void Update(IEnumerable<TDocument> entities)
    {
        foreach (var entity in entities)
        {
            Collection.FindOneAndUpdate(_databaseContext.SessionHandle,
                filter: Builders<TDocument>.Filter.Eq(IdInDbName, GetIdValue(entity)), new ObjectUpdateDefinition<TDocument>(entity));
        }
    }

    #endregion

    #region Delete

    public void Delete(object id)
    {
        Collection.FindOneAndDelete(_databaseContext.SessionHandle,
            filter: Builders<TDocument>.Filter.Eq(IdInDbName, id));
    }

    public void Delete(TDocument entity)
    {
        Collection.DeleteOne(_databaseContext.SessionHandle, document => document == entity  );
    }

    public void Delete(params TDocument[] entities)
    {
        foreach (var entity in entities)
        {
            Collection.DeleteOne(_databaseContext.SessionHandle, document => document == entity);
        }
    }

    public void Delete(IEnumerable<TDocument> entities)
    {
        foreach (var entity in entities)
        {
            Collection.DeleteOne(_databaseContext.SessionHandle, document => document == entity);
        }
    }

    #endregion

    #region PagedList

    public IPagedList<TDocument> GetPagedList(
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null, 
        int pageIndex = 0, 
        int pageSize = 20)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return orderBy is not null
            ? orderBy(query).ToPagedList(pageIndex, pageSize)
            : query.ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TDocument>> GetPagedListAsync(
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null,
        int pageIndex = 0, 
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return orderBy is not null
            ? orderBy(query).ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken)
            : query.ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
    }

    public IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<TDocument, TResult>> selector, 
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null,
        int pageIndex = 0, int pageSize = 20) 
        where TResult : class
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return orderBy is not null
            ? orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize)
            : query.Select(selector).ToPagedList(pageIndex, pageSize);
    }

    public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<TDocument, TResult>> selector, 
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null,
        int pageIndex = 0, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default) 
        where TResult : class
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
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
    
    /// <summary> Returns paged collection of the items </summary>
    /// <remarks>Pagination for MongoDB is using AggregateFacet</remarks>
    /// <param name="pageSize"></param>
    /// <param name="filter"></param>
    /// <param name="sorting"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageIndex"></param>
    public async Task<IPagedList<TDocument>> GetPagedListAsync
    (
        int pageIndex,
        int pageSize,
        FilterDefinition<TDocument> filter,
        SortDefinition<TDocument> sorting,
        CancellationToken cancellationToken)
    {
        var countFacet = AggregateFacet.Create(CountFacetName,
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));

        var dataFacet = AggregateFacet.Create(DataFacetName,
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(sorting),
                PipelineStageDefinitionBuilder.Skip<TDocument>((pageIndex - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
            }));


        var aggregation = await Collection.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync(cancellationToken: cancellationToken);

        var count = aggregation.First()
            .Facets.First(x => x.Name == CountFacetName)
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;

        var totalPages = (int)Math.Ceiling((double)count! / pageSize);

        var data = aggregation.First()
            .Facets.First(x => x.Name == DataFacetName)
            .Output<TDocument>();

        return data.ToPagedList(pageIndex, pageSize, totalPages, count);
    }

    #endregion

    #region GetFirstOrDefault

    public TDocument? GetFirstOrDefault(Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).FirstOrDefault()
            : IAsyncCursorSourceExtensions.FirstOrDefault(query);
    }

    public async Task<TDocument?> GetFirstOrDefaultAsync(Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).FirstOrDefault()!
            : await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    #endregion

    #region GetAll

    public IQueryable<TDocument> GetAll()
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return query;
    }

    public IQueryable<TResult> GetAll<TResult>(Expression<Func<TDocument, TResult>> selector)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return query.Select(selector);
    }

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TDocument, TResult>> selector, 
        Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return query.Select(selector);
    }

    public IQueryable<TDocument> GetAll(
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query)
            : query;
    }

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TDocument, TResult>> selector, 
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null
            ? orderBy(query).Select(selector)
            : query.Select(selector);
    }

    public async Task<IList<TDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TDocument, TResult>> selector, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TDocument>> GetAllAsync(
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            return orderBy(query).ToList();
        }

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TDocument, TResult>> selector, 
        Expression<Func<TDocument, bool>>? predicate = null, 
        Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).ToList()
            : await query.Select(selector).ToListAsync(cancellationToken: cancellationToken);
    }
    
    #endregion

    #region Count

    public int Count(Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.Count()
            : query.Count(predicate);
    }

    public Task<int> CountAsync(Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.CountAsync(cancellationToken: cancellationToken)
            : query.CountAsync(predicate, cancellationToken: cancellationToken);
    }

    public long LongCount(Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.LongCount()
            : query.LongCount(predicate);
    }

    public Task<long> LongCountAsync(Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.LongCountAsync(cancellationToken: cancellationToken)
            : query.LongCountAsync(predicate, cancellationToken: cancellationToken);
    }

    #endregion

    #region Exist

    public bool Exists(Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? IAsyncCursorSourceExtensions.Any(query)
            : query.Any(predicate);
    }

    public Task<bool> ExistsAsync(Expression<Func<TDocument, bool>>? selector = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return selector is null
            ? query.AnyAsync(cancellationToken: cancellationToken)
            : query.AnyAsync(selector, cancellationToken: cancellationToken);
    }

    #endregion

    #region Max

    public T? Max<T>(Expression<Func<TDocument, T>> selector, Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.Max(selector)
            : query.Where(predicate).Max(selector);
    }

    public Task<T> MaxAsync<T>(Expression<Func<TDocument, T>> selector, Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.MaxAsync(selector, cancellationToken)
            : query.Where(predicate).MaxAsync(selector, cancellationToken);
    }

    #endregion

    #region Min

    public T? Min<T>(Expression<Func<TDocument, T>> selector, Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.Min(selector)
            : query.Where(predicate).Min(selector);
    }

    public Task<T> MinAsync<T>(Expression<Func<TDocument, T>> selector, Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.MinAsync(selector, cancellationToken)
            : query.Where(predicate).MinAsync(selector, cancellationToken);

    }

    #endregion

    #region Average

    public decimal Average(Expression<Func<TDocument, decimal>> selector, Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.Average(selector)
            : query.Where(predicate).Average(selector);
    }

    public Task<decimal> AverageAsync(Expression<Func<TDocument, decimal>> selector, Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.AverageAsync(selector, cancellationToken)
            : query.Where(predicate).AverageAsync(selector, cancellationToken);
    }

    #endregion

    #region Sum

    public decimal Sum(Expression<Func<TDocument, decimal>> selector, Expression<Func<TDocument, bool>>? predicate = null)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.Sum(selector)
            : query.Where(predicate).Sum(selector);
    }

    public Task<decimal> SumAsync(Expression<Func<TDocument, decimal>> selector, Expression<Func<TDocument, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return predicate is null
            ? query.SumAsync(selector, cancellationToken)
            : query.Where(predicate).SumAsync(selector, cancellationToken);
    }

    #endregion
}