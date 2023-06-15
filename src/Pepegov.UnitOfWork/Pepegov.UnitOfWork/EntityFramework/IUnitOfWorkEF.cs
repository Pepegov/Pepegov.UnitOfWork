using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework;

public interface IUnitOfWorkEF<out TContext> : IUnitOfWorkEF where TContext : DbContext
{
    TContext DbContext { get; }

    Task<int> SaveChangesAsync(params IUnitOfWorkEF[] unitOfWorks);
}

public interface IUnitOfWorkEF : IDisposable
{
    IRepositoryEF<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);

    Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false);

    IDbContextTransaction BeginTransaction(bool useIfExists = false);

    void SetAutoDetectChanges(bool value);

    SaveChangesResult LastSaveChangesResult { get; }
}