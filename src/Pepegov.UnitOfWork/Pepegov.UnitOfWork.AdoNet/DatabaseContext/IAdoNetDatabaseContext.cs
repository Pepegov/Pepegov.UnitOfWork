using System.Data;

namespace Pepegov.UnitOfWork.AdoNet.DatabaseContext;

public interface IAdoNetDatabaseContext : IDatabaseContext 
{
    IDbConnection DbConnection { get; }
}