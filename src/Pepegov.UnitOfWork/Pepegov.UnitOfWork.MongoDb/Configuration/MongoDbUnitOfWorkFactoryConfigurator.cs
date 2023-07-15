using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;

namespace Pepegov.UnitOfWork.MongoDb.Configuration;

public class MongoDbUnitOfWorkFactoryConfigurator : IMongoDbUnitOfWorkFactoryConfigurator
{
    private readonly IUnitOfWorkRegistrationContext _context;

    public IMongoDatabaseContext MongoDatabaseContext { get; private set; }

    public MongoDbUnitOfWorkFactoryConfigurator(IUnitOfWorkRegistrationContext context)
    {
        _context = context;
    }

    private IDatabaseBuilder GetDatabaseBuilder(IDatabaseSettings databaseSettings)
    {
        var nameSelector = _context.GetService<ICollectionNameSelector>();
        if (nameSelector is null)
        {
            nameSelector = new DefaultCollectionNameSelector();
        }

        var loggerDatabaseBuilder = _context.GetService<ILogger<IDatabaseBuilder>>();
        if(loggerDatabaseBuilder is null)
        {
            loggerDatabaseBuilder = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<IDatabaseBuilder>();
        }
        return new DatabaseBuilder(loggerDatabaseBuilder, databaseSettings, nameSelector);
    }

    private IDatabaseSettings GetDatabaseSetting(Action<DatabaseSettings>? databaseFactory)
    {
        DatabaseSettings? databaseSettings = new DatabaseSettings();
        if (databaseFactory is null)
        { 
            databaseSettings = _context.GetService<IDatabaseSettings>() as DatabaseSettings;
            ArgumentNullException.ThrowIfNull(databaseSettings, "DatabaseSettings != null");
        }

        databaseFactory?.Invoke(databaseSettings);
        return databaseSettings;
    }

    public void DatabaseContext(Action<DatabaseSettings>? databaseFactory = null)
    {
        var settings = GetDatabaseSetting(databaseFactory);
        var builder = GetDatabaseBuilder(settings);
        var logger = _context.GetService<ILogger<IMongoDatabaseContext>>();
        if(logger is null)
        {
            logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<IMongoDatabaseContext>();
        }
        MongoDatabaseContext = new MongoDatabaseContext(logger, builder);
    }
}