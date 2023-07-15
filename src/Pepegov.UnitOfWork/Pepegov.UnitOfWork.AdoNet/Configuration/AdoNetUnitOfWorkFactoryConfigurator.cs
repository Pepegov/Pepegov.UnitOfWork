using System.Data;
using Pepegov.UnitOfWork.Configuration;

namespace Pepegov.UnitOfWork.AdoNet.Configuration;

public class AdoNetUnitOfWorkFactoryConfigurator : IAdoNetUnitOfWorkFactoryConfigurator
{
    private readonly IUnitOfWorkRegistrationContext _context;

    public IDbConnection? DbConnection { get; private set; }

    public AdoNetUnitOfWorkFactoryConfigurator(IUnitOfWorkRegistrationContext context)
    {
        _context = context;
    }
    
    public void DatabaseContext(Func<IDbConnection> configureDbConnection)
    {
        DbConnection = configureDbConnection.Invoke();
    }
}