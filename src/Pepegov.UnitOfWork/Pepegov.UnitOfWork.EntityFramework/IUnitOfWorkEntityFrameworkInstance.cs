using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pepegov.UnitOfWork.Entityes;
using Pepegov.UnitOfWork.EntityFramework.Repository;

namespace Pepegov.UnitOfWork.EntityFramework;

public interface IUnitOfWorkEntityFrameworkInstance<out TContext> : IUnitOfWorkEntityFrameworkInstance where TContext : DbContext
{
    TContext DbContext { get; }
}

public interface IUnitOfWorkEntityFrameworkInstance : IUnitOfWorkTrackingInstance, IRepositoryEntityFrameworkInstanceFactory, IDisposable
{
    void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);

    void SetAutoDetectChanges(bool value);

    SaveChangesResult LastSaveChangesResult { get; }
}