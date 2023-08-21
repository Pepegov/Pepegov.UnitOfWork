using System;
using Microsoft.Extensions.Logging;
using Pepegov.UnitOfWork.MongoDb.Database;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public interface IMongoDbUnitOfWorkFactoryConfigurator
{
    void DatabaseContext(Action<DatabaseSettings>? databaseFactory = null);
}