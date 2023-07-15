﻿using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Exceptions;
using Pepegov.UnitOfWork.Extensions;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.Repository;
using Pepegov.UnitOfWork.Search.FuzzySearch;

namespace Pepegov.UnitOfWork.MongoDb.Repository
{
    /// <summary>
    /// Generic repository for wrapper of mongoDb Collection
    /// </summary>
    /// <typeparam name="TDocument">The type of the entity.</typeparam>
    public sealed class RepositoryMongo<TDocument> : BaseRepository<TDocument>, IRepositoryMongo<TDocument> where TDocument : class
    {
        private const string DataFacetName = "dataFacet";
        private const string CountFacetName = "countFacet";
        
        private readonly IMongoDatabaseContext _databaseContext;
        private readonly ILogger<IUnitOfWorkMongoInstance> _logger;

        private const string IdInDbName = "_id";

        /// <summary>
        /// Returns instance of new <see cref="IRepository{TEntity}"/>
        /// </summary>
        /// <param name="databaseContext"></param>
        /// <param name="logger"></param>
        /// <param name="writeConcern">default is WriteConcern.WMajority</param>
        /// <param name="readConcern">default is ReadConcern.Local</param>
        /// <param name="readPreference">default is ReadPreference.Primary</param>
        public RepositoryMongo(IMongoDatabaseContext databaseContext,
            ILogger<IUnitOfWorkMongoInstance> logger,
            ICollectionNameSelector collectionNameSelector,
            WriteConcern? writeConcern = null,
            ReadConcern? readConcern = null,
            ReadPreference? readPreference = null) : base(collectionNameSelector)
        {
            _databaseContext = databaseContext;
            _logger = logger;

            Collection = GetOrCreateCollection(databaseContext.DatabaseBuilder, writeConcern, readConcern, readPreference);
        }
        
        /// <summary>
        /// MongoDb collection (<see cref="IMongoCollection{TDocument}"/>
        /// </summary>
        public IMongoCollection<TDocument> Collection { get; }

        /// <summary>Gets the namespace of the collection.</summary>
        public CollectionNamespace CollectionNamespace => Collection.CollectionNamespace;

        /// <summary>Gets the database.</summary>
        public IMongoDatabase Database => Collection.Database;

        /// <summary>Gets the document serializer.</summary>
        public IBsonSerializer<TDocument> DocumentSerializer => Collection.DocumentSerializer;

        /// <summary>Gets the index manager.</summary>
        public IMongoIndexManager<TDocument> Indexes => Collection.Indexes;

        /// <summary>Gets the settings.</summary>
        public MongoCollectionSettings Settings => Collection.Settings;

        #region privates

        /// <summary>
        /// Returns new instance of <see cref="IMongoCollection{TDocument}"/>
        /// </summary>
        /// <typeparam name="T">type of collection</typeparam>
        /// <param name="name">collection name in MongoDb</param>
        /// <param name="writeConcern">default is WriteConcern.WMajority</param>
        /// <param name="readConcern">default is ReadConcern.Local</param>
        /// <param name="readPreference">default is ReadPreference.Primary</param>
        /// <returns>MongoDb collection</returns>
        private IMongoCollection<T> GetCollection<T>(string name,
            WriteConcern? writeConcern = null,
            ReadConcern? readConcern = null,
            ReadPreference? readPreference = null)
            => Collection.Database.GetCollection<T>(name)
                .WithWriteConcern(writeConcern ?? WriteConcern.WMajority)
                .WithReadConcern(readConcern ?? ReadConcern.Local)
                .WithReadPreference(readPreference ?? ReadPreference.Primary);

        /// <summary>
        /// Returns collection of items (getting already exists or create before and return)
        /// </summary>
        /// <param name="databaseBuilder"></param>
        /// <param name="writeConcern">default is WriteConcern.WMajority</param>
        /// <param name="readConcern">default is ReadConcern.Local</param>
        /// <param name="readPreference">default is ReadPreference.Primary</param>
        /// <returns>MongoDb collection</returns>
        private IMongoCollection<TDocument> GetOrCreateCollection(IDatabaseBuilder databaseBuilder,
            WriteConcern? writeConcern = null,
            ReadConcern? readConcern = null,
            ReadPreference? readPreference = null)
        {
            var mongoDb = databaseBuilder.Build();
            if (mongoDb.GetCollection<BsonDocument>(EntityName) == null)
            {
                mongoDb.CreateCollection(EntityName);
            }

            return mongoDb.GetCollection<TDocument>(EntityName)
                .WithWriteConcern(writeConcern ?? WriteConcern.WMajority)
                .WithReadConcern(readConcern ?? ReadConcern.Local)
                .WithReadPreference(readPreference ?? ReadPreference.Primary);
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

        /// <summary> Returns paged collection of the items </summary>
        /// <remarks>Pagination for MongoDB is using AggregateFacet</remarks>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="sorting"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="pageIndex"></param>
        public async Task<IPagedList<TDocument>> GetPagedAsync
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
                : query.FirstOrDefault();
        }

        public async Task<TDocument?> GetFirstOrDefaultAsync(Expression<Func<TDocument, bool>>? predicate = null, 
            Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
        {
            IMongoQueryable<TDocument> query = Collection.AsQueryable();
            
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return orderBy is not null
                ? orderBy(query).FirstOrDefault()!
                : await query.FirstOrDefaultAsync();
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

        public async Task<IList<TDocument>> GetAllAsync()
        {
            IMongoQueryable<TDocument> query = Collection.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<IList<TResult>> GetAllAsync<TResult>(
            Expression<Func<TDocument, TResult>> selector)
        {
            IMongoQueryable<TDocument> query = Collection.AsQueryable();
            return await query.Select(selector).ToListAsync();
        }

        public async Task<IList<TDocument>> GetAllAsync(
            Expression<Func<TDocument, bool>>? predicate = null, 
            Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
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

            return await query.ToListAsync();
        }

        public async Task<IList<TResult>> GetAllAsync<TResult>(
            Expression<Func<TDocument, TResult>> selector, 
            Expression<Func<TDocument, bool>>? predicate = null, 
            Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null)
        {
            IMongoQueryable<TDocument> query = Collection.AsQueryable();
            
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return orderBy is not null
                ? orderBy(query).Select(selector).ToList()
                : await query.Select(selector).ToListAsync();
        }

        public async Task<IList<TDocument>> GetAllWithFuzzySearchAsync(string searchQuery, Func<TDocument, string> searchProperty, Expression<Func<TDocument, bool>>? predicate = null,
            Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>? orderBy = null, int allowedMistakeDistance = 300)
        {
            IMongoQueryable<TDocument> query = Collection.AsQueryable();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            var distanceLevenshtein = new DistanceLevenshtein(new char[] { ' ', '-' });
            distanceLevenshtein.SetData(query.Select(
                item => new Tuple<string, string>(searchProperty(item), searchProperty(item))).ToList());

            var result = distanceLevenshtein.Search(searchQuery);
            var matches = new List<string>();
            matches.AddRange(result
                .Where(i => i.Item3 <= allowedMistakeDistance)
                .Select(i => i.Item1).ToList());

            query = query
                .Where(item => matches.
                    Any(i => searchProperty(item).ToLower().Contains(i)));

            return orderBy is null ? await query.ToListAsync() : orderBy(query).ToList();
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
                ? query.Any()
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
}