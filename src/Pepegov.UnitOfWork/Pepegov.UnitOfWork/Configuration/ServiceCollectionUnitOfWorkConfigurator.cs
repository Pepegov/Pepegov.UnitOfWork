using Microsoft.Extensions.DependencyInjection;

namespace Pepegov.UnitOfWork.Configuration;

/// <summary>
/// Верхний конфигуратор AddUnitOfWork
/// </summary>
public class ServiceCollectionUnitOfWorkConfigurator : IUnitOfWorkRegistrationConfigurator
{
    public IServiceCollection ServiceCollection { get; }

    public ServiceCollectionUnitOfWorkConfigurator(IServiceCollection collection)
    {
        ServiceCollection = collection ?? throw new ArgumentNullException(nameof(collection));

        ServiceCollection.AddSingleton<IUnitOfWorkRegistrationContext>(provider => new UnitOfWorkRegistrationContext(provider));
    }

    public void AddUnitOfWorkInstance<T>(T unitOfWorkFactory) where T : IRegistrationUnitOfWorkFactory
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory, nameof(unitOfWorkFactory));
        ServiceCollection.AddScoped<IUnitOfWorkInstance>(pr => 
            unitOfWorkFactory.CreateUnitOfWorkInstance(pr.GetService<IUnitOfWorkRegistrationContext>() 
                                                       ?? throw new InvalidOperationException($"failed get service {nameof(IUnitOfWorkRegistrationContext)}")));
    }
}