using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.Exceptions;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Repository;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public class MongoDbUnitOfWorkDependenciesFactoryConfigurator : IMongoDbUnitOfWorkDependenciesFactoryConfigurator
{
    private readonly IServiceCollection _collection;

    public MongoDbUnitOfWorkDependenciesFactoryConfigurator(IServiceCollection collection)
    {
        _collection = collection;
        ArgumentNullException.ThrowIfNull(_collection, "ServiceCollection != null");
    }

    /// <summary>
    /// Registers the unit of work given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <param name="implementationFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddInstance(Func<IServiceProvider, IDatabaseSettings> implementationFactory)
    {
        ArgumentNullException.ThrowIfNull(implementationFactory, "implementationFactory != null");

        TryAddLogging();
        
        _collection.TryAddScoped(typeof(IDatabaseSettings), implementationFactory);
        
        TryAddCollectionNameSelector<DefaultCollectionNameSelector>();
        _collection.TryAddScoped<IDatabaseBuilder, DatabaseBuilder>();
        _collection.TryAddScoped<IMongoDatabaseContext, MongoDatabaseContext>();
        
        _collection.TryAddScoped<IUnitOfWorkMongoInstance, UnitOfWorkMongoDbInstance>();
        _collection.TryAddScoped<IRepositoryMongoFactory, UnitOfWorkMongoDbInstance>();
    }

    /// <summary>
    /// Registers the unit of work given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>
    /// The same service collection so that multiple calls can be chained.
    /// </returns>
    /// <remarks>
    /// This method only support one db context, if been called more than once, will throw exception.
    /// </remarks>
    /// <param name="applyConfiguration"></param>
    public void AddInstance(Action<DatabaseSettings> applyConfiguration)
    {
        var mongoDbSettings = new DatabaseSettings();
        applyConfiguration(mongoDbSettings);

        TryAddLogging();
            
        _collection.TryAddScoped<IDatabaseSettings>(_ => mongoDbSettings);
        
        TryAddCollectionNameSelector<DefaultCollectionNameSelector>();
        _collection.TryAddScoped<IDatabaseBuilder, DatabaseBuilder>();
        _collection.TryAddScoped<IMongoDatabaseContext, MongoDatabaseContext>();
        
        _collection.TryAddScoped<IUnitOfWorkMongoInstance, UnitOfWorkMongoDbInstance>();
        _collection.TryAddScoped<IRepositoryMongoFactory, UnitOfWorkMongoDbInstance>();
    }

    /// <summary>
    /// Registers the unit of work given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method only support one db context, if been called more than once, will throw exception.
    /// </remarks>
    /// <param name="configurationSection"></param>
    /// <exception cref="UnitOfWorkArgumentNullException"></exception>
    public void AddInstance(IConfigurationSection configurationSection)
    {
        var mongoDbSettings = configurationSection.Get<DatabaseSettings>();

        if (mongoDbSettings == null)
        {
            throw new UnitOfWorkArgumentNullException(nameof(DatabaseSettings));
        }

        TryAddLogging();

        _collection.TryAddScoped<IDatabaseSettings>(_ => mongoDbSettings);

        TryAddCollectionNameSelector<DefaultCollectionNameSelector>();
        _collection.TryAddScoped<IDatabaseBuilder, DatabaseBuilder>();
        _collection.TryAddScoped<IMongoDatabaseContext, MongoDatabaseContext>();
        
        _collection.TryAddScoped<IUnitOfWorkMongoInstance, UnitOfWorkMongoDbInstance>();
        _collection.TryAddScoped<IRepositoryMongoFactory, UnitOfWorkMongoDbInstance>();
    }

    //TODO test this method
    public void AddCollectionNameSelector<TCollectionNameSelector>() where TCollectionNameSelector : ICollectionNameSelector
    {
        _collection.TryAddScoped(typeof(ICollectionNameSelector), typeof(TCollectionNameSelector));
    }

    private void TryAddCollectionNameSelector<TCollectionNameSelector>() where TCollectionNameSelector : ICollectionNameSelector
    {
        var nameSelectorDescriptor = _collection.FirstOrDefault(x => x.ServiceType == typeof(ICollectionNameSelector));
        if (nameSelectorDescriptor is null)
        {
            _collection.TryAddScoped(typeof(ICollectionNameSelector), typeof(TCollectionNameSelector));
        }
    }
    
    private void TryAddLogging()
    {
        var loggerFactoryDescriptor = _collection.FirstOrDefault(x => x.ServiceType == typeof(ILoggerFactory));
        if (loggerFactoryDescriptor is null)
        {
            _collection.AddLogging();
        }
    }
}