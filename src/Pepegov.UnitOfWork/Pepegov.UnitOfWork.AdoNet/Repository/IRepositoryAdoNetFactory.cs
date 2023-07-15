using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.AdoNet.Repository;

public interface IRepositoryAdoNetFactory : IRepositoryFactory
{
    IRepositoryAdoNet<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
}