using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Test.Core;
using Pepegov.UnitOfWork.EntityFramework.Test.Database;
using Pepegov.UnitOfWork.EntityFramework.Test.Entity;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Test;

public class InsertDeleteTests : TestBase
{
    private IUnitOfWorkEntityFrameworkInstance _entityFrameworkInstance;

    [TearDown]
    public void TearDown()
    {
        _entityFrameworkInstance.Dispose();
    }
    
    [SetUp]
    protected override async void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>();
        
        services.AddUnitOfWork(x =>
        {
            x.UsingEntityFramework((_, configurator) =>
            {
                configurator.DatabaseContext<TestDbContext>();
            });
        });

        var provider = services.BuildServiceProvider();

        var dbContext = provider.GetService<TestDbContext>();
        ArgumentNullException.ThrowIfNull(dbContext, "dbContext != null");
        await SeedDataAsync(dbContext);
        
        var unitOfWorkManager = provider.GetService<IUnitOfWorkManager>()!;
        _entityFrameworkInstance = unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();
    }

    [Test]
    [TestCase("Bob1", "German")]
    public async Task InsertDeleteWithTransaction(string firstName, string lastName)
    {
        await _entityFrameworkInstance.BeginTransactionAsync();
        var personRepository = _entityFrameworkInstance.GetRepository<Person>();
        var person = new Person() { Name = firstName, LastName = lastName };
        
        var insertResult = await personRepository.InsertAsync(person);
        await _entityFrameworkInstance.SaveChangesAsync();
        var entity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == insertResult.Entity.Id);
        Assert.That(_entityFrameworkInstance.LastSaveChangesResult.IsOk && entity is not null);
        
        personRepository.Delete(insertResult.Entity);
        await _entityFrameworkInstance.SaveChangesAsync();
        Assert.That(_entityFrameworkInstance.LastSaveChangesResult.IsOk && entity is not null);
        
        await _entityFrameworkInstance.CommitTransactionAsync();
        
        var deletedEntity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == insertResult.Entity.Id);
        Assert.That(deletedEntity is null);
    }
}