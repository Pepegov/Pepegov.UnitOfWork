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

    public void SaveAllChanges()
    {
        foreach (var instance in Instances)
        {
            if (instance is IUnitOfWorkTrackingInstance trackingInstance)
            {
                trackingInstance.SaveChanges();
            }
        }
    }

    public async Task SaveAllChangesAsync()
    {
        foreach (var instance in Instances)
        {
            if (instance is IUnitOfWorkTrackingInstance trackingInstance)
            {
                await trackingInstance.SaveChangesAsync();
            }
        }
    }
}