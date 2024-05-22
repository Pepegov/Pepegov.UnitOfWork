namespace Pepegov.UnitOfWork.Repository;

public interface IRepositoryQueryable<TEntity> where TEntity : class
{
    TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> func);
    Task<TResult> QueryAsync<TResult>(Func<IQueryable<TEntity>, Task<TResult>> func, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Query(Action<IQueryable<TEntity>> func);
}