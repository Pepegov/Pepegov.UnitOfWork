using Microsoft.Extensions.DependencyInjection;

namespace Pepegov.UnitOfWork.Configuration;

public interface IUnitOfWorkRegistrationConfigurator
{
    IServiceCollection ServiceCollection { get; }
    public void AddUnitOfWorkInstance<T>(T unitOfWorkFactory)
        where T : IRegistrationUnitOfWorkFactory;
}