using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;
using Pepegov.UnitOfWork.EntityFramework.Test.Database;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Test;

public class ConfigurationTests
{
    private IServiceProvider _serviceProvider;

    [TearDown]
    public void TearDown()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        } 
    }
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>();
        /*
        string migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name!;
        string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=qweQWE123;Database=Pepegov.UnitOfWork.Test";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(migrationsAssembly));
        });
        */
        
        services.AddUnitOfWork(x =>
        {
            x.UsingEntityFramework((_, configurator) =>
            {
                configurator.DatabaseContext<TestDbContext>();
            }, dependencies =>
            {
                dependencies.AddInstance<TestDbContext>();
                //TODO:fix that
                //dependencies.AddCustomRepository<Person, CustomEfRepository<Person>>();
            });
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public void DatabaseContextTest()
    {
        var unitOfWorkManager = _serviceProvider.GetService<IUnitOfWorkManager>();
        Assert.That(unitOfWorkManager is UnitOfWorkManager);   
    }
    
    [Test]
    public void AddInstanceTest()
    {
        var instanceFactory = _serviceProvider.GetService<IRepositoryEntityFrameworkInstanceFactory>();
        Assert.That(instanceFactory is IUnitOfWorkEntityFrameworkInstance<TestDbContext>);
        
        var instance = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance>();
        Assert.That(instance is IUnitOfWorkEntityFrameworkInstance<TestDbContext>);
        
        var instanceContext = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance<TestDbContext>>();
        Assert.That(instanceContext != null);
    }
    
}