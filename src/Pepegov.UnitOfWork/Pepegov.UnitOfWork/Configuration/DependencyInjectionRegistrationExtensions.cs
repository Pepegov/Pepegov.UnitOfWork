using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.Configuration;
using Pepegov.UnitOfWork.Exceptions;

namespace Pepegov.UnitOfWork;

public static class DependencyInjectionRegistrationExtensions
{
    /// <summary>
    /// Добавляет UnitOfWorkManager и его инструменты
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    /// <exception cref="UnitOfWorkConfigurationException"></exception>
    public static IServiceCollection AddUnitOfWork(this IServiceCollection collection, Action<IUnitOfWorkRegistrationConfigurator> configure = null)
    {
        if (collection.Any(d => d.ServiceType == typeof(IUnitOfWorkManager)))
        {
            throw new UnitOfWorkConfigurationException(
                "AddUnitOfWork() was already called and may only be called once per container.");
        }

        var configurator = new ServiceCollectionUnitOfWorkConfigurator(collection);
        
        configure?.Invoke(configurator);
        collection.AddScoped<IUnitOfWorkManager>(pr 
            => new UnitOfWorkManager(pr.GetServices<IUnitOfWorkInstance>()));
        
        return collection;
    }
}