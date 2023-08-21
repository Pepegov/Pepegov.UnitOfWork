using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    #region Find

    TEntity? Find(params object[] keyValues);

    ValueTask<TEntity?> FindAsync(params object[] keyValues);

    ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken);

    #endregion
    
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