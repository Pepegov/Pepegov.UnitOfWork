namespace Pepegov.UnitOfWork.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    #region Insert

    void Insert(params TEntity[] entities);

    void Insert(IEnumerable<TEntity> entities);

    Task InsertAsync(params TEntity[] entities);

    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

    #endregion
   
    #region Update

    void Update(TEntity entity);

    void Update(params TEntity[] entities);

    void Update(IEnumerable<TEntity> entities);

    #endregion

    #region Delete

    void Delete(object id);

    void Delete(TEntity entity);

    void Delete(params TEntity[] entities);

    void Delete(IEnumerable<TEntity> entities);

    #endregion
}