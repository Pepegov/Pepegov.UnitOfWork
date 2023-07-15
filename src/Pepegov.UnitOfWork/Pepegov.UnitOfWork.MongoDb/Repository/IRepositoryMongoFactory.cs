using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Repository;

public interface IRepositoryMongoFactory
{
    /// <summary>
    /// Returns repository wrapper for MongoDb collection
    /// </summary>
    /// <typeparam name="TDocument">type of Document</typeparam>
    /// <param name="writeConcern">default </param>
    /// <param name="readConcern"></param>
    /// <param name="readPreference"></param>
    public IRepositoryMongo<TDocument> GetRepository<TDocument>(
        WriteConcern? writeConcern = null,
        ReadConcern? readConcern = null,
        ReadPreference? readPreference = null) where TDocument : class;
}