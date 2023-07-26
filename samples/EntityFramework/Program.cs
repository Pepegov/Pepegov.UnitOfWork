using Microsoft.Extensions.DependencyInjection;
using Pepegov.UnitOfWork;
using Pepegov.UnitOfWork.EntityFramework;
using Pepegov.UnitOfWork.EntityFramework.Configuration;
using UnitOfWorkEntityFrameworkSample.Database;

var services = new ServiceCollection();

// Configure context
services.AddDbContext<ApplicationDbContext>();
        
//Configure UnitOfWorkManager 
services.AddUnitOfWork(x =>
{
    x.UsingEntityFramework((context, configurator) =>
    {
        configurator.DatabaseContext<ApplicationDbContext>();
    });
});
var provider = services.BuildServiceProvider();

//Seeding context
var dbContext = provider.GetService<ApplicationDbContext>();
ArgumentNullException.ThrowIfNull(dbContext, "dbContext != null");
DatabaseInitializer.Seed(dbContext);
        
//Get instance
var unitOfWorkManager = provider.GetService<IUnitOfWorkManager>()!;
var entityFrameworkInstance = unitOfWorkManager.GetInstance<IUnitOfWorkEntityFrameworkInstance>();

//Transaction
entityFrameworkInstance.BeginTransaction();
var personRepository = entityFrameworkInstance.GetRepository<Person>();

var id = Guid.NewGuid();
personRepository.Insert(new Person() { Id = id, Name = "My name" });

try
{
    //same id has throw exception
    personRepository.Insert(new Person() { Id = id, Name = "My name" });
}
catch (Exception)
{
    entityFrameworkInstance.RollbackTransaction();
}

entityFrameworkInstance.CommitTransaction();

//Tracking & unstacking entity's
var trackingEntity = personRepository.GetFirstOrDefault(predicate: x => x.Id == id, disableTracking: false);
if (trackingEntity != null)
    trackingEntity.LastName += "Tracking";
entityFrameworkInstance.SaveChanges();

var unstackingEntity = personRepository.GetFirstOrDefault(predicate: x => x.Id == id);
entityFrameworkInstance.SetAutoDetectChanges(false);
