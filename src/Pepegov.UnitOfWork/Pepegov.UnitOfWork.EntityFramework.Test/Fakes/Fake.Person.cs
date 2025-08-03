using Bogus;
using Person = Pepegov.UnitOfWork.EntityFramework.Test.Entity.Person;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Fakes;

public static partial class Fake
{
    public static class PersonFaker
    {
        public static Faker<Person> CreatePersonFaker()
        {
            return new Faker<Person>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Age, f => f.Random.Int(0, 100));
        }
    }

}