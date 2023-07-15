using Microsoft.Extensions.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public interface IMongoDbUnitOfWorkDependenciesFactoryConfigurator
{
    void AddInstance(Func<IServiceProvider, IDatabaseSettings> implementationFactory);
    void AddInstance(Action<DatabaseSettings> applyConfiguration);
    void AddInstance(IConfigurationSection configurationSection);

    void AddCollectionNameSelector<TCollectionNameSelector>()
        where TCollectionNameSelector : ICollectionNameSelector;
}