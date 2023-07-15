using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.EntityFramework.Configuration;

public static class UnitOfWorkRegistrationConfiguratorExtensions
{
    public static void UsingEntityFramework(this IUnitOfWorkRegistrationConfigurator configurator,
        Action<IUnitOfWorkRegistrationContext, IEntityFrameworkUnitOfWorkFactoryConfigurator> configureInstance,
        Action<IEntityFrameworkUnitOfWorkDependenciesFactoryConfigurator>? configureDependencies = null)
    {
        configurator.AddUnitOfWorkInstance(new EntityFrameworkRegistrationUnitOfWorkFactory(configureInstance));
        if (configureDependencies is not null)
        {
            var dependenciesFactory =
                new EntityFrameworkUnitOfWorkDependenciesFactoryConfigurator(configurator.ServiceCollection);
            configureDependencies(dependenciesFactory);
        }
    }
}