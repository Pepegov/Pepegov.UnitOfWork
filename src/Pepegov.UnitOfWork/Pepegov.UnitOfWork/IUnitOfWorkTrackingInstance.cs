using System.Threading;
using System.Threading.Tasks;

namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkTrackingInstance : IUnitOfWorkInstance
{
    void SaveChanges();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}