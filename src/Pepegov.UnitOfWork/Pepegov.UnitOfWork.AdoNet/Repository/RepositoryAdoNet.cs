using Pepegov.UnitOfWork.AdoNet.DatabaseContext;
using Pepegov.UnitOfWork.Repository;

namespace Pepegov.UnitOfWork.AdoNet.Repository;

public class RepositoryAdoNet<TEntity> : IRepositoryAdoNet<TEntity> where TEntity : class
{
    private readonly AdoNetDatabaseContext _context;
    private readonly string _tableName; 
    
    public RepositoryAdoNet(AdoNetDatabaseContext context)
    {
        _context = context;
        _tableName = GetType().GetGenericArguments()[0].Name;
    }

    #region Find

    public TEntity? Find(params object[] keyValues)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TEntity?> FindAsync(params object[] keyValues)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Insert

    public void Insert(params TEntity[] entities)
    {
        var command = _context.CreateCommand();
    }

    public void Insert(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(params TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Update

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Update(params TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public void Update(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Delete

    public void Delete(object id)
    {
        throw new NotImplementedException();
    }

    public void Delete(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(params TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    #endregion
}