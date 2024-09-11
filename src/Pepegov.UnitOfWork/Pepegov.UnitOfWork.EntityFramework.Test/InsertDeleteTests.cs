using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using Pepegov.UnitOfWork.EntityFramework.Test.Database;

namespace Pepegov.UnitOfWork.EntityFramework.Test;

public class InsertDeleteTests
{
    private IUnitOfWorkEntityFrameworkInstance entityFrameworkInstance;

    [TearDown]
    public void TearDown()
    {
        entityFrameworkInstance.Dispose();
    }
    
    [SetUp]
    public async Task Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>();
        
        services.AddUnitOfWork(x =>
        {
            x.UsingEntityFramework((context, configurator) =>
            {
                configurator.DatabaseContext<ApplicationDbContext>();
            });
        });

        var provider = services.BuildServiceProvider();

        var dbContext = provider.GetService<ApplicationDbContext>();
        ArgumentNullException.ThrowIfNull(dbContext, "dbContext != null");
        DatabaseInitializer.Seed(dbContext);
        
        var unitOfWorkManager = provider.GetService<IUnitOfWorkManager>()!;
        entityFrameworkInstance = unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();
    }

    [Test]
    [TestCase("Bob1", "Kerman")]
    public async Task InsertDeleteWithTransaction(string firstName, string lastName)
    {
        await entityFrameworkInstance.BeginTransactionAsync();
        var personRepository = entityFrameworkInstance.GetRepository<Person>();
        var person = new Person() { Name = firstName, LastName = lastName };
        
        var insertResult = await personRepository.InsertAsync(person);
        await entityFrameworkInstance.SaveChangesAsync();
        var entity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == insertResult.Entity.Id);
        Assert.That(entityFrameworkInstance.LastSaveChangesResult.IsOk && entity is not null);
        
        personRepository.Delete(insertResult.Entity);
        await entityFrameworkInstance.SaveChangesAsync();
        Assert.That(entityFrameworkInstance.LastSaveChangesResult.IsOk && entity is not null);
        
        await entityFrameworkInstance.CommitTransactionAsync();
        
        var deletedEntity = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == insertResult.Entity.Id);
        Assert.That(deletedEntity is null);
    }
}