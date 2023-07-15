using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.EntityFramework.Repository
{
    public interface IRepositoryEntityFrameworkInstanceFactory : IRepositoryFactory
    {
        IRepositoryEntityFramework<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
    }
}