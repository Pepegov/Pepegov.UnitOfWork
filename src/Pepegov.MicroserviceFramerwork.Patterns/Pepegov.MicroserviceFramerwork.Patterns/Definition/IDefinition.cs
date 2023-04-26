using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pepegov.MicroserviceFramerwork.Patterns.Definition
{
    public interface IDefinition
    {
        bool Enabled { get; }

        void ConfigureServicesAsync(IServiceCollection services, WebApplicationBuilder builder);

        void ConfigureApplicationAsync(WebApplication app);
    }
}