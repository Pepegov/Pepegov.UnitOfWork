using Microsoft.Extensions.DependencyInjection;

namespace Pepegov.UnitOfWork.Configuration;

public class UnitOfWorkRegistrationContext : IUnitOfWorkRegistrationContext
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkRegistrationContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }
}