namespace Pepegov.UnitOfWork.Configuration;

public interface IRegistrationUnitOfWorkFactory
{
    IUnitOfWorkInstance CreateUnitOfWorkInstance(IUnitOfWorkRegistrationContext context);
}