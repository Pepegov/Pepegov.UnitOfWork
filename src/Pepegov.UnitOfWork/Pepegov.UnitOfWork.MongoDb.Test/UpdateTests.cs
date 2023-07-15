using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.MongoDb.Configuration;
using Pepegov.UnitOfWork.MongoDb.Database;
using Pepegov.UnitOfWork.MongoDb.Test.Entityes;

namespace Pepegov.UnitOfWork.MongoDb.Test;

public class UpdateTests
{
    private IUnitOfWorkMongoInstance _unitOfWorkMongoDbInstance;
    
    [SetUp]
    public void Setup()
    {
        _unitOfWorkMongoDbInstance = TestHelper.GetUnitOfWorkMongoDbInstanceStandart;
    }
    
    [Test]
    [TestCase("Margo", "Kerman")]
    public async Task UpdateTest(string firstName, string lastName)
    {
        var personRepository = _unitOfWorkMongoDbInstance.GetRepository<Person>();
        var person = new Person() { Name = firstName, LastName = lastName };
        
        await personRepository.InsertAsync(person);
        
        person.Name += "Updated"; 
        personRepository.Update(person);
        
        var entityBefore = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == person.Id);
        
        Assert.That(entityBefore is not null && entityBefore.Name == person.Name);
        
        personRepository.Delete(person);
    }
}