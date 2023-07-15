namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkManager
{
    public IEnumerable<IUnitOfWorkInstance> Instances { get; }
    public void AddInstance(IUnitOfWorkInstance instance); 
    public TInstance GetInstance<TInstance>() where TInstance : IUnitOfWorkInstance;
    public void SaveAllChanges();
    public Task SaveAllChangesAsync();
}