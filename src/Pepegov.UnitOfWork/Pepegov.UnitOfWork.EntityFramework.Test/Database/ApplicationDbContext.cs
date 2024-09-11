using Microsoft.EntityFrameworkCore;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Person> Persones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlite("Data Source=ef.test.sqlite");
        //optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=qweQWE123;Database=Pepegov.UnitOfWork.Test");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}