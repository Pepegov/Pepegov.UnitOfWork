using System.Text;

namespace Pepegov.UnitOfWork.Test;

public class ICollectionNameSelectorTests
{
    private ICollectionNameSelector _collectionNameSelector;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _collectionNameSelector = new DefaultCollectionNameSelector();
    }

    [Test]
    public void Test()
    {
        var entityType = typeof(MyClass);
        var entityName = new StringBuilder(entityType.Name);
        if (entityType.IsGenericType)
        {
            foreach (var genericArgument in entityType.GetGenericArguments())
            {
                entityName.Append(genericArgument.Name);
            }
        }
        entityName.Replace("`1", "");

        var name = _collectionNameSelector.GetCollectionName(entityName.ToString());
        Assert.Pass();
    }
    
    private class MyGenericClass<T>
    {
    }
    
    private class MyClass
    {
    }
}