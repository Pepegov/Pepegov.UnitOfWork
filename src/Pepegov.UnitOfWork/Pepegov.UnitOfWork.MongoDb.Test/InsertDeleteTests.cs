using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.MongoDb.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Test.Entityes;

namespace Pepegov.UnitOfWork.MongoDb.Test;

public class InsertDeleteTests
{
    private IUnitOfWorkMongoInstance _mongoInstance;
    
    [SetUp]
    public void Setup()
    {
        _mongoInstance = TestHelper.GetUnitOfWorkMongoDbInstanceStandard;
    }

    [Test]
    public async Task InsertDeleteTest()
    {
        var firstName = "insertDeleteTestFirstName";
        var lastName = "insertDeleteTestLastName";
        
        var personRepository = _mongoInstance.GetRepository<Person>();
        var person = new Person() { Name = firstName, LastName = lastName };
        
        await personRepository.InsertAsync(person);

        var entity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == person.Id);

        Assert.That(_mongoInstance.LastSaveChangesResult!.IsOk && entity is not null);
        
        personRepository.Delete(person);
        var deletedEntity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == person.Id);
        Assert.That(deletedEntity is null);
    }
    
    [Test]
    public async Task InsertManyDeleteManyTest()
    {
        var firstName = "insertDeleteTestFirstName";
        var lastName = "insertDeleteTestLastName";
        var count = 10;
        
        var personRepository = _mongoInstance.GetRepository<Person>();

        List<Person> persones = new();
        for (int i = 0; i <= count; i++)
        {
            persones.Add(new Person() { Name = firstName + i, LastName = lastName + i});    
        }
        
        await personRepository.InsertAsync(persones);

        var entities = await personRepository.GetAllAsync(predicate: x => x.LastName.StartsWith(lastName));

        Assert.That(_mongoInstance.LastSaveChangesResult!.IsOk && entities is not null && entities.Count == persones.Count);
        
        personRepository.Delete(persones);
        var deletedEntities = await personRepository.GetAllAsync(predicate: x => x.LastName.StartsWith(lastName));
        Assert.That(deletedEntities is null || deletedEntities.Count == 0);
    }
}