using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pepegov.UnitOfWork.EntityFramework.Database;

public interface IEntityFrameworkDatabaseContext : IDatabaseContext
{
    void SaveChanges();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    DbContext DbContext { get; }
}