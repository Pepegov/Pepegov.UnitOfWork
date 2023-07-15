using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.AdoNet.Configuration;

public static class UnitOfWorkRegistrationConfiguratorExtensions
{
    public static void UsingAdoNet(this IUnitOfWorkRegistrationConfigurator configurator,
        Action<IUnitOfWorkRegistrationContext, IAdoNetUnitOfWorkFactoryConfigurator> configure = null)
    {
        configurator.AddUnitOfWorkInstance(new AdoNetRegistrationUnitOfWorkFactory(configure));
    }
}