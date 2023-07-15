using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public static class UnitOfWorkRegistrationConfiguratorExtensions
{
    public static void UsingMongoDb(this IUnitOfWorkRegistrationConfigurator configurator,
        Action<IUnitOfWorkRegistrationContext, IMongoDbUnitOfWorkFactoryConfigurator>? configureInstance = null,
        Action<IMongoDbUnitOfWorkDependenciesFactoryConfigurator>? configureDependencies = null)
    {
        if(configureInstance is not null)
        {
            configurator.AddUnitOfWorkInstance(new MongoDbRegistrationUnitOfWorkFactory(configureInstance));
        }
        if (configureDependencies is not null)
        {
            var dependenciesFactory =
                new MongoDbUnitOfWorkDependenciesFactoryConfigurator(configurator.ServiceCollection);
            configureDependencies(dependenciesFactory);
        }
    }
}