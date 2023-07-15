using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.AdoNet.Configuration;

public class AdoNetRegistrationUnitOfWorkFactory : IRegistrationUnitOfWorkFactory
{
    private readonly Action<IUnitOfWorkRegistrationContext, IAdoNetUnitOfWorkFactoryConfigurator> _configure;

    public AdoNetRegistrationUnitOfWorkFactory(Action<IUnitOfWorkRegistrationContext, IAdoNetUnitOfWorkFactoryConfigurator> configure)
    {
        _configure = configure;
    }

    public IUnitOfWorkInstance CreateUnitOfWorkInstance(IUnitOfWorkRegistrationContext context)
    {
        var configurator = new AdoNetUnitOfWorkFactoryConfigurator(context);
        
        _configure?.Invoke(context, configurator);
        
        var instance = new UnitOfWorkAdoNetInstance(configurator.DbConnection, context);

        return instance;
    }
}