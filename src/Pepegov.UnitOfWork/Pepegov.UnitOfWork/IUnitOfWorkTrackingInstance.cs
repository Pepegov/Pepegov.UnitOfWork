namespace Pepegov.UnitOfWork;

public interface IUnitOfWorkTrackingInstance : IUnitOfWorkInstance
{
    void SaveChanges();
    Task SaveChangesAsync();
}