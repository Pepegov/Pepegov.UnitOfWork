using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    public IEnumerable<IUnitOfWorkInstance> Instances { get; }
    
    public UnitOfWorkManager(IEnumerable<IUnitOfWorkInstance> instances)
    {
        Instances = instances;
    }
    
    public void AddInstance(IUnitOfWorkInstance instance)
    {
        Instances.ToList().Add(instance);
    }

    public TInstance GetInstance<TInstance>() where TInstance : IUnitOfWorkInstance
    {
        var instance = Instances.FirstOrDefault(x => x is TInstance);
        ArgumentNullException.ThrowIfNull(instance, $"{nameof(instance)} in {nameof(GetInstance)}");
        return (TInstance)instance;
    }

    #region SaveChanges

    public void SaveChanges()
    {
        foreach (var instance in Instances)
        {
            if (instance is IUnitOfWorkTrackingInstance trackingInstance)
            {
                trackingInstance.SaveChanges();
            }
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var instance in Instances)
        {
            if (instance is IUnitOfWorkTrackingInstance trackingInstance)
            {
                await trackingInstance.SaveChangesAsync(cancellationToken);
            }
        }
    }

    #endregion

    #region CommitTransactions

    public void CommitTransactions()
    {
        foreach (var instance in Instances)
        {
            instance.CommitTransaction();
        }
    }

    public async Task CommitTransactionsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var instance in Instances)
        {
            await instance.CommitTransactionAsync(cancellationToken);
        }
    }

    #endregion

    #region RollbackTransactions

    public void RollbackTransactions()
    {
        foreach (var instance in Instances)
        {
            instance.RollbackTransaction();
        }
    }

    public async Task RollbackTransactionsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var instance in Instances)
        {
            await instance.RollbackTransactionAsync(cancellationToken);
        }
    }

    #endregion
}