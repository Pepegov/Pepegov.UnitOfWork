using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkManager
{
    public IEnumerable<IUnitOfWorkInstance> Instances { get; }
    public void AddInstance(IUnitOfWorkInstance instance); 
    public TInstance GetInstance<TInstance>() where TInstance : IUnitOfWorkInstance;
    
    public void SaveChanges();
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    public void CommitTransactions();
    public Task CommitTransactionsAsync(CancellationToken cancellationToken = default);
    public void RollbackTransactions();
    public Task RollbackTransactionsAsync(CancellationToken cancellationToken = default);
}