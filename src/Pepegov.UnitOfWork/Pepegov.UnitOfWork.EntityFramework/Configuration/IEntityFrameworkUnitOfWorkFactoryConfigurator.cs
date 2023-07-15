using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public interface IEntityFrameworkUnitOfWorkFactoryConfigurator : IUnitOfWorkFactoryConfigurator
{
    void DatabaseContext<TDbContext>() where TDbContext : DbContext;
}