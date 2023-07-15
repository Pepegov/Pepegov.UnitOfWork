using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;
using Pepegov.UnitOfWork.EntityFramework.Test.Database;

namespace Pepegov.UnitOfWork.EntityFramework.Test;

public class ConfigurationTests
{
    private IServiceProvider _serviceProvider;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>();
        
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
    public void AddInstanceTest()
    {
        var instanceFactory = _serviceProvider.GetService<IRepositoryEntityFrameworkInstanceFactory>();
        Assert.That(instanceFactory is not null && instanceFactory is IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>);
        
        var instance = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance>();
        Assert.That(instance is not null && instance is IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>);
        
        var instanceContext = _serviceProvider.GetService<IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>>();
        Assert.That(instanceContext is not null && instanceContext is IUnitOfWorkEntityFrameworkInstance<ApplicationDbContext>);
    }
    
}