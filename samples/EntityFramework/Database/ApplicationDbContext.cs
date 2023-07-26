using Microsoft.EntityFrameworkCore;

namespace UnitOfWorkEntityFrameworkSample.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Person> Persones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ef.test.sqlite");
        base.OnConfiguring(optionsBuilder);
    }
}