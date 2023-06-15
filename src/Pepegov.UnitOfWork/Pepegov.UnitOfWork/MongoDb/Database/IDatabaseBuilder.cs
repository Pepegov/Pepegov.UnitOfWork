using Calabonga.UnitOfWork.MongoDb;
using MongoDB.Driver;

namespace Pepegov.UnitOfWork.MongoDb.Database;

/// <summary>
/// Database builder from database settings
/// </summary>
public interface IDatabaseBuilder
{
    ICollectionNameSelector CollectionNameSelector { get; }

    IMongoDatabase Build();

    IMongoClient? Client { get; }

    IDatabaseSettings Settings { get; }
}