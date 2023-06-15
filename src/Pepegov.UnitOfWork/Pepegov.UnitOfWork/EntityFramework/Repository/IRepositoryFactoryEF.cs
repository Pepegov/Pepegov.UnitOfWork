namespace Pepegov.UnitOfWork.EntityFramework.Repository
{
    public interface IRepositoryEFFactory
    {
        IRepositoryEF<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
    }
}