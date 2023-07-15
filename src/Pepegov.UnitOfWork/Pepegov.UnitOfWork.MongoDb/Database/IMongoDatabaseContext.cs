using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Database;

public interface IMongoDatabaseContext : IDatabaseContext
{
    IMongoDatabase MongoDatabase { get; }
    IDatabaseBuilder DatabaseBuilder { get; }
    IClientSessionHandle SessionHandle { get; }
}