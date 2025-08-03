namespace Pepegov.UnitOfWork.EntityFramework.Test.Entity;

public class Person
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public int Age { get; set; }
}