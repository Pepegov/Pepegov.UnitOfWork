namespace Pepegov.UnitOfWork;

/// <summary>
/// Collection name selector for MongoDb entities
/// </summary>
public interface ICollectionNameSelector
{
    /// <summary>
    /// Returns a name for entity by it type
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns>name</returns>
    string GetCollectionName(string typeName);
}