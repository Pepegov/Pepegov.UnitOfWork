using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.MongoDb.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Repository;

namespace Pepegov.UnitOfWork.MongoDb.Test;

public class ConfigurationTests
{
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUnitOfWork(x =>
        {
            x.UsingMongoDb((_, configurator) =>
            {
                configurator.DatabaseContext(settings =>
                {
                    //settings.ConnectionString = "mongodb://localhost:27017/?readPreference=primary&ssl=false&directConnection=true";
                    settings.Credential = new CredentialSettings("root", "qweQWE123");
                    settings.ApplicationName = "Pepegov.UnitOfWork.MongoDb.Test";
                    settings.DatabaseName = "pepegov_unitofwork_mongoDb_test";
                    settings.Hosts = new[] { "localhost" };
                    settings.MongoDbPort = 27017;
                    settings.VerboseLogging = true;
                    settings.DirectConnection = true;
                });
            });
        });
    }
    
    //TODO: add other settings test
    [Test]
    public void AddInstanceWithApplySettingsArgumentTest()
    {
        var services = new ServiceCollection();
        services.AddUnitOfWork(x =>
        {
            x.UsingMongoDb(configureDependencies: dependencyConfigurator =>
            {
                dependencyConfigurator.AddInstance(settings =>
                {
                    //settings.ConnectionString = "mongodb://localhost:27017/?readPreference=primary&ssl=false&directConnection=true";
                    settings.Credential = new CredentialSettings("root", "qweQWE123");
                    settings.ApplicationName = "Pepegov.UnitOfWork.MongoDb.Test";
                    settings.DatabaseName = "pepegov_unitofwork_mongoDb_test";
                    settings.Hosts = new[] { "localhost" };
                    settings.MongoDbPort = 27016;
                    settings.VerboseLogging = true;
                    settings.DirectConnection = true;
                });
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        
        var nameSelector = serviceProvider.GetService<ICollectionNameSelector>();
        Assert.That(nameSelector is DefaultCollectionNameSelector);
        
        var databaseSettings = serviceProvider.GetService<IDatabaseSettings>();
        Assert.That(databaseSettings is DatabaseSettings);

        var databaseBuilder = serviceProvider.GetService<IDatabaseBuilder>();
        Assert.That(databaseBuilder is DatabaseBuilder);

        var databaseContext = serviceProvider.GetService<IMongoDatabaseContext>();
        Assert.That(databaseContext is MongoDatabaseContext);
        
        var instanceFactory = serviceProvider.GetService<IRepositoryMongoFactory>();
        Assert.That(instanceFactory is UnitOfWorkMongoDbInstance);
        
        var instance = serviceProvider.GetService<IUnitOfWorkMongoInstance>();
        Assert.That(instance is UnitOfWorkMongoDbInstance);
    }
}