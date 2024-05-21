namespace Pepegov.UnitOfWork.Repository;

public interface IRepositoryQueryable<TEntity> where TEntity : class
{
    TResult Query<TResult>(Func<IQueryable, TResult> func);
    Task<TResult> QueryAsync<TResult>(Func<IQueryable, Task<TResult>> func, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Query(Action<IQueryable> func);
}