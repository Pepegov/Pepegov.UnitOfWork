using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.MongoDb.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;

namespace Pepegov.UnitOfWork.MongoDb.Test;

public static class TestHelper
{
    public static IUnitOfWorkMongoInstance GetUnitOfWorkMongoDbInstanceStandart
        => _serviceProvider.GetRequiredService<IUnitOfWorkMongoInstance>();

    public static IUnitOfWorkMongoInstance GetUnitOfWorkMongoDbInstanceWithReplica
        => _serviceProvider.GetRequiredService<IUnitOfWorkManager>().GetInstance<IUnitOfWorkMongoInstance>();

    private static readonly IServiceProvider _serviceProvider;
    
    static TestHelper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUnitOfWork(x =>
        {
            x.UsingMongoDb((context, configurator) =>
            {
                configurator.DatabaseContext(settings =>
                {
                    settings.ConnectionString = "mongodb://localhost:27017/?authSource=admin&readPreference=primary&ssl=false&directConnection=true";
                    settings.DatabaseName = "pepegov_unitofwork_mongoDb_replica_test";
                });
            }, factory =>
            {
                factory.AddInstance(settings =>
                {
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

        _serviceProvider = services.BuildServiceProvider();
    }
}