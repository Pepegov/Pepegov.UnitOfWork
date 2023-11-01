using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pepegov.UnitOfWork.Configuration;

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

        var collectionNameSelector = context.GetService<ICollectionNameSelector>();
        if (collectionNameSelector is null)
        {
            collectionNameSelector = new DefaultCollectionNameSelector();
        }
        
        var instance = new UnitOfWorkMongoDbInstance(loggerInstance, collectionNameSelector, configurator.MongoDatabaseContext);

        return instance;
    }
}