namespace Pepegov.UnitOfWork.MongoDb.Test.Entityes;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }

    public Person()
    {
        Id = Guid.NewGuid();
    }
}