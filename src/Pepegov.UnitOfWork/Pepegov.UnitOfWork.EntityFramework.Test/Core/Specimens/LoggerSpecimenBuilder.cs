using AutoFixture.Kernel;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Core.Specimens;

public class LoggerSpecimenBuilder : ISpecimenBuilder
{
    private static readonly Lazy<ILoggerFactory> LoggerFactoryLazy = new Lazy<ILoggerFactory>(CreateLogger);

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ILogger<>))
        {
            return new NoSpecimen(); // indicate that current specimen builder cannot create requested object
        }

        var forType = type.GenericTypeArguments[0];
        var loggerType = typeof(Logger<>).MakeGenericType(forType);
        return Activator.CreateInstance(loggerType, new object[] { LoggerFactoryLazy.Value })!;
    }

    private static ILoggerFactory CreateLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        return new LoggerFactory()
            .AddSerilog(Log.Logger);
    }
}