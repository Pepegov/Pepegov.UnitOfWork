using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.MongoDb.Repository
{
    /// <summary>
    /// Generic repository for wrapper of mongoDb Collection
    /// </summary>
    /// <typeparam name="TDocument">The type of the entity.</typeparam>
    public partial class RepositoryMongo<TDocument> : BaseRepository<TDocument>, IRepositoryMongo<TDocument> where TDocument : class
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
    }
}