using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.AdoNet.Repository;

public interface IRepositoryAdoNet<TEntity> : IRepository<TEntity> where TEntity : class
{
}