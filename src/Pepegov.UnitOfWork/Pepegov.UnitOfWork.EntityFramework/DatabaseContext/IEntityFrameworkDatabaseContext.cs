using Microsoft.EntityFrameworkCore;

namespace Pepegov.UnitOfWork.EntityFramework.DatabaseContext;

public interface IEntityFrameworkDatabaseContext : IDatabaseContext
{
    void SaveChanges();
    Task SaveChangesAsync();
    DbContext DbContext { get; }
}