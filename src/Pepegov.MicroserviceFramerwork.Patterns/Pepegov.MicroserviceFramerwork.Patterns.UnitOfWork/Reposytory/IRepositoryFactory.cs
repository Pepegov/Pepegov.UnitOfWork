namespace Pepegov.MicroserviceFramerwork.Patterns.UnitOfWork.Reposytory
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
    }
}