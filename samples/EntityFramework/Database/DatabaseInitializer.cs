using Microsoft.EntityFrameworkCore;

namespace UnitOfWorkEntityFrameworkSample.Database;

public static class DatabaseInitializer
{
    public static void Seed(DbContext context)
    {
        context.Database.EnsureCreated();
        //await context.Database.MigrateAsync();
    }
}