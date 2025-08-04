using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pepegov.UnitOfWork.EntityFramework.Test.Core.DbConverters;
using Pepegov.UnitOfWork.EntityFramework.Test.Entity;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Database;

public class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                // JSON conversion for dictionary and list types
                if (property.ClrType == typeof(Dictionary<string, string>))
                {
                    property.SetValueConverter(new EntityToJsonConverter<Dictionary<string, string>>());
                }
                else if (property.ClrType == typeof(List<string>))
                {
                    property.SetValueConverter(new EntityToJsonConverter<List<string>>());
                }

                // Decimal type conversion for SQLite
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetValueConverter(
                        new ValueConverter<decimal, double>(
                            to => Convert.ToDouble(to),
                            from => Convert.ToDecimal(from)));
                }

                // DateTimeOffset conversion for SQLite
                if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTimeOffset, DateTime>(
                            to => to.UtcDateTime,
                            from => DateTime.SpecifyKind(from, DateTimeKind.Utc)));
                }
            }
        }
    }
}