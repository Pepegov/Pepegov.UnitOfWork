using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public class EntityFrameworkUnitOfWorkDependenciesFactoryConfigurator : IEntityFrameworkUnitOfWorkDependenciesFactoryConfigurator
{
    private readonly IServiceCollection _serviceCollection;

    public EntityFrameworkUnitOfWorkDependenciesFactoryConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void AddInstance<TDbContext>() where TDbContext : DbContext
    {
        _serviceCollection.AddScoped<IRepositoryEntityFrameworkInstanceFactory, UnitOfWorkEntityFrameworkInstance<TDbContext>>();
        _serviceCollection.AddScoped<IUnitOfWorkEntityFrameworkInstance, UnitOfWorkEntityFrameworkInstance<TDbContext>>(); 
        _serviceCollection.AddTransient<IUnitOfWorkEntityFrameworkInstance<TDbContext>, UnitOfWorkEntityFrameworkInstance<TDbContext>>();
    }
    
    public void AddCustomRepository<TEntity, TRepository>()
        where TEntity : class
        where TRepository : class, IRepositoryEntityFramework<TEntity>
    {
        _serviceCollection.AddScoped<IRepositoryEntityFramework<TEntity>, TRepository>();
    }
}