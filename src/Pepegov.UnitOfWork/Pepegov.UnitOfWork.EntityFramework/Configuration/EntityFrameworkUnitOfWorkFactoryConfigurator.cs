using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public class EntityFrameworkUnitOfWorkFactoryConfigurator : IEntityFrameworkUnitOfWorkFactoryConfigurator
{
    private readonly IUnitOfWorkRegistrationContext _context;
    public DbContext? DbContext { get; private set; }

    public EntityFrameworkUnitOfWorkFactoryConfigurator(IUnitOfWorkRegistrationContext context)
    {
        _context = context;
    }

    public void DatabaseContext<TDbContext>() where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(_context,$"{nameof(_context)} in {nameof(DatabaseContext)}");
        
        DbContext = _context.CreateScope().ServiceProvider.GetService<TDbContext>();
    }
}