using Microsoft.EntityFrameworkCore;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Database;

public static class DatabaseInitializer
{
    public static void Seed(DbContext context)
    {
        context.Database.EnsureCreated();
        //await context.Database.MigrateAsync();
    }
}