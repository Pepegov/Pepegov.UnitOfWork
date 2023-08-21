using System;
using Microsoft.EntityFrameworkCore;
using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public class EntityFrameworkRegistrationUnitOfWorkFactory : IRegistrationUnitOfWorkFactory
{
    private readonly Action<IUnitOfWorkRegistrationContext, IEntityFrameworkUnitOfWorkFactoryConfigurator> _configure;
    
    public EntityFrameworkRegistrationUnitOfWorkFactory(
        Action<IUnitOfWorkRegistrationContext, IEntityFrameworkUnitOfWorkFactoryConfigurator> configure)
    {
        _configure = configure;
    }
    
    public IUnitOfWorkInstance CreateUnitOfWorkInstance(IUnitOfWorkRegistrationContext context)
    {
        var configurator = new EntityFrameworkUnitOfWorkFactoryConfigurator(context);
        
        _configure?.Invoke(context, configurator);
        
        var instance = new UnitOfWorkEntityFrameworkInstance<DbContext>(configurator.DbContext);

        return instance;
    }
}