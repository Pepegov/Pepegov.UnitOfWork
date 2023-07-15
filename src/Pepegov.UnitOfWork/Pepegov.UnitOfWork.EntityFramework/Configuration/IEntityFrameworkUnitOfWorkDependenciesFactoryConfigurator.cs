using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public interface IEntityFrameworkUnitOfWorkDependenciesFactoryConfigurator : IUnitOfWorkFactoryConfigurator
{
    void AddInstance<TDbContext>() where TDbContext : DbContext;
    public void AddCustomRepository<TEntity, TRepository>()
        where TEntity : class
        where TRepository : class, IRepositoryEntityFramework<TEntity>;
}