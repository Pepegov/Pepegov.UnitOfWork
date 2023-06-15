using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IRepositoryEFFactory, UnitOfWorkEF<TContext>>();
            services.AddScoped<IUnitOfWorkEF, UnitOfWorkEF<TContext>>();
            services.AddScoped<IUnitOfWorkEF<TContext>, UnitOfWorkEF<TContext>>();

            return services;
        }

        public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
            where TEntity : class
            where TRepository : class, IRepositoryEF<TEntity>
        {
            services.AddScoped<IRepositoryEF<TEntity>, TRepository>();

            return services;
        }
    }
}