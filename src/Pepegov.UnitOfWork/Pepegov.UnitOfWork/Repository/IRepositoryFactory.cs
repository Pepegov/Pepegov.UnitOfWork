namespace Pepegov.UnitOfWork.Repository;

public interface IRepositoryFactory
{
    IRepository<T> GetRepository<T>() where T : class;
}