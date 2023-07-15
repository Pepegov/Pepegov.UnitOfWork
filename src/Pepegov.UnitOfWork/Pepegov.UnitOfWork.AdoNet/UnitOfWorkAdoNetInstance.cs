using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.AdoNet.DatabaseContext;
using Pepegov.UnitOfWork.AdoNet.Repository;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.AdoNet;

public class UnitOfWorkAdoNetInstance : BaseUnitOfWorkInstance, IRepositoryAdoNetFactory
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<Type, object>? _repositories;

    public UnitOfWorkAdoNetInstance(IDbConnection dbConnection, IServiceProvider serviceProvider) : base(new AdoNetDatabaseContext(dbConnection))
    {
        _serviceProvider = serviceProvider;
    }
    
    #region Dispose

    

    #endregion
    public override void Dispose()
    {
        DatabaseContext.Dispose();
        base.LastSaveChangesResult = null;
        GC.SuppressFinalize(this);
    }

    #region GetRepository

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();


        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new RepositoryAdoNet<TEntity>(DatabaseContext as AdoNetDatabaseContext);
        }

        return (IRepositoryAdoNet<TEntity>)_repositories[type];
    }

    public IRepositoryAdoNet<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();

        if (hasCustomRepository)
        {
            var customRepo = _serviceProvider.GetService<IRepositoryAdoNet<TEntity>>();
            if (customRepo != null)
            {
                return customRepo;
            }
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new RepositoryAdoNet<TEntity>(DatabaseContext as AdoNetDatabaseContext);
        }

        return (IRepositoryAdoNet<TEntity>)_repositories[type];
    }

    #endregion
    
}