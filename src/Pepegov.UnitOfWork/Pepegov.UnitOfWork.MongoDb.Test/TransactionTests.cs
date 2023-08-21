using Pepegov.UnitOfWork.Exceptions;
using Pepegov.UnitOfWork.MongoDb.Test.Entityes;

namespace Pepegov.UnitOfWork.MongoDb.Test;

public class TransactionTests
{
    private IUnitOfWorkMongoInstance _mongoInstance;
    private IUnitOfWorkMongoInstance _mongoTransactionNotSupportedInstance;
    
    [SetUp]
    public void Setup()
    {
        _mongoInstance = TestHelper.GetUnitOfWorkMongoDbInstanceWithReplica;
        _mongoTransactionNotSupportedInstance = TestHelper.GetUnitOfWorkMongoDbInstanceStandard;
        if (!_mongoInstance.EnsureReplicationSetReady())
        {
            throw new UnitOfWorkTransactionNotSupported();
        }
    }

    [Test]
    public async Task TransactionNotSupportedTest()
    {
        Assert.ThrowsAsync<NotSupportedException>(() => _mongoTransactionNotSupportedInstance.BeginTransactionAsync());
    }

    [Test] 
    [TestCase("transactionRollbackPersonFirstName", "transactionRollbackPersonLastName")]
    public async Task TransactionRollbackTest(string firstName, string lastName)
    {
        await _mongoInstance.BeginTransactionAsync();
        
        var personRepository = _mongoInstance.GetRepository<Person>();
        var person = new Person() { Name = firstName, LastName = lastName };
        
        await personRepository.InsertAsync(person);

        try
        {
            await personRepository.InsertAsync(person);
        }
        catch (Exception ex)
        {
            await _mongoInstance.RollbackTransactionAsync();
            Assert.That(_mongoInstance.LastSaveChangesResult!.IsOk);
            return;
        }

        Assert.Fail();
    }
    
    [Test] 
    public async Task TransactionCommitTest()
    {
        await _mongoInstance.BeginTransactionAsync();
        
        var personRepository = _mongoInstance.GetRepository<Person>();
        var person1 = new Person() { Name = "transactionCommitPersonFirstName", LastName = "transactionCommitPersonLastName" };
        var person2 = new Person() { Name = "transactionCommitPersonFirstNameTwo", LastName = "transactionCommitPersonLastNameTwo" };

        await personRepository.InsertAsync(person1);
        await personRepository.InsertAsync(person2);
        personRepository.Delete(person1);

        await _mongoInstance.CommitTransactionAsync();

        person1 = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == person1.Id);
        person2 = await personRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == person2.Id);

        Assert.That(person1 is null);
        Assert.That(person2 is not null);
        
        personRepository.Delete(person2);
    }
}