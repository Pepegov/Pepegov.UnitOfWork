namespace UnitOfWorkEntityFrameworkSample.Database;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LastName { get; set; }
}