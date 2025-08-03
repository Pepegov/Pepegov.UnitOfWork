using Pepegov.UnitOfWork.EntityFramework.Test.Core;
using Pepegov.UnitOfWork.EntityFramework.Test.Entity;
using Pepegov.UnitOfWork.EntityFramework.Test.Fakes;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Test;

public class RepositoryQuerybleExpressionTests : TestBase
{
    #region Min

    [Test]
    public void Min_ShouldReturnMin_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act
        var personMinAge = personRepository.Min(s => s.Age);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Min(x => x.Age)));
    }
    
    [Test]
    public void Min_ShouldReturnMin_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        SeedData(persons.ToArray());
        
        // Act
        var personMinAge = personRepository.Min(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Where(w => w.Age > 10).Min(x => x.Age)));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnMin_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMinAge = await personRepository.MinAsync(s => s.Age);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Min(x => x.Age)));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnMin_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMinAge = await personRepository.MinAsync(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Where(w => w.Age > 10).Min(x => x.Age)));
    }

    #endregion
}