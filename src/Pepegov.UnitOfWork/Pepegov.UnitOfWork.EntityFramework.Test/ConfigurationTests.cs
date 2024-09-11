using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;
using Pepegov.UnitOfWork.EntityFramework.Test.Database;

namespace Pepegov.UnitOfWork.EntityFramework.Test;

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
        services.AddDbContext<ApplicationDbContext>();
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
            x.UsingEntityFramework((context, configurator) =>
            {
                configurator.DatabaseContext<ApplicationDbContext>();
            }, dependencies =>
            {
                dependencies.AddInstance<ApplicationDbContext>();
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
        Assert.That(instanceFactory is IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>);
        
        var instance = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance>();
        Assert.That(instance is IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>);
        
        var instanceContext = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>>();
        Assert.That(instanceContext != null);
    }
    
}