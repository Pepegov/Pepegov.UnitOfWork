using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Database;

public interface IDatabaseBuilder
{
    IDatabaseSettings Settings { get; }
    IMongoDatabase Build();
    IMongoClient Client { get; }
}