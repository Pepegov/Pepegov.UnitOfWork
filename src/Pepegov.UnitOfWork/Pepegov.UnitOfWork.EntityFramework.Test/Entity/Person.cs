using System.ComponentModel.DataAnnotations;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Entity;

public class Person
{
    public Guid Id { get; set; }
    [MaxLength(100)]
    
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string LastName { get; set; }
    public int Age { get; set; }
}