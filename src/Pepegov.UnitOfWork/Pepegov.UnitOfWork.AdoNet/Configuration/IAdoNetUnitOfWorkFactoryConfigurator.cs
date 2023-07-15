using System.Data;
using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.AdoNet.Configuration;

public interface IAdoNetUnitOfWorkFactoryConfigurator : IUnitOfWorkFactoryConfigurator
{
    void DatabaseContext(Func<IDbConnection> configureDbConnection);
}