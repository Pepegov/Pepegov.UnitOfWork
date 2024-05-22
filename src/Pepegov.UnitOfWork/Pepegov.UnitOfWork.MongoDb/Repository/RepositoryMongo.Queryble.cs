using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pepegov.UnitOfWork.MongoDb.Repository;

public partial class RepositoryMongo<TDocument> where TDocument : class
{
    public TResult Query<TResult>(Func<IQueryable<TDocument>, TResult> func)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return func.Invoke(query);
    }

    public Task<TResult> QueryAsync<TResult>(Func<IQueryable<TDocument>, Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        return func.Invoke(query);
    }

    public IQueryable<TDocument> Query(Action<IQueryable<TDocument>> func)
    {
        IMongoQueryable<TDocument> query = Collection.AsQueryable();
        func.Invoke(query);
        return query;
    }
}