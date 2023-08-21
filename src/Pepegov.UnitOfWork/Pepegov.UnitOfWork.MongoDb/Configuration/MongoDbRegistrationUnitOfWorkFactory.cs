using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public class MongoDbRegistrationUnitOfWorkFactory : IRegistrationUnitOfWorkFactory
{
    private readonly Action<IUnitOfWorkRegistrationContext, IMongoDbUnitOfWorkFactoryConfigurator> _configure;

    public MongoDbRegistrationUnitOfWorkFactory(Action<IUnitOfWorkRegistrationContext, IMongoDbUnitOfWorkFactoryConfigurator> configure)
    {
        _configure = configure;
    }

    public IUnitOfWorkInstance CreateUnitOfWorkInstance(IUnitOfWorkRegistrationContext context)
    {
        var configurator = new MongoDbUnitOfWorkFactoryConfigurator(context);
        
        _configure?.Invoke(context, configurator);

        var loggerInstance = context.GetService<ILogger<IUnitOfWorkMongoInstance>>();
        if (loggerInstance is null)
        {
            loggerInstance = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<IUnitOfWorkMongoInstance>();
        }
        
        var loggerContext = context.GetService<ILogger<IMongoDatabaseContext>>();
        if (loggerContext is null)
        {
            loggerContext = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<IMongoDatabaseContext>();
        }

        var collectionNameSelector = context.GetService<ICollectionNameSelector>();
        if (collectionNameSelector is null)
        {
            collectionNameSelector = new DefaultCollectionNameSelector();
        }
        
        var instance = new UnitOfWorkMongoDbInstance(loggerInstance, loggerContext, collectionNameSelector, configurator.MongoDatabaseContext);

        return instance;
    }
}