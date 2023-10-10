using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Pepegov.UnitOfWork.Exceptions;

namespace Pepegov.UnitOfWork.Repository;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    private readonly ICollectionNameSelector _collectionNameSelector;
    
    protected readonly PropertyInfo EntityIdPropertyInfo;
    protected readonly string EntityName;
    protected readonly Type EntityType;
    
    protected BaseRepository(ICollectionNameSelector collectionNameSelector)
    {
        _collectionNameSelector = collectionNameSelector;
        EntityType = typeof(TEntity);
        EntityIdPropertyInfo = TryGetIdProperty();
        EntityName = GetInternalName();
    }
    
    protected object? GetIdValue(TEntity entity)
        => EntityIdPropertyInfo.GetValue(entity);
    
    protected virtual PropertyInfo TryGetIdProperty()
    {
        PropertyInfo? key;
        key = EntityType.GetProperties().FirstOrDefault(p => p
            .GetCustomAttribute(typeof(KeyAttribute)) is not null);
        if (key is not null)
        {
            return key;
        }

        key = EntityType.GetProperties().FirstOrDefault(p => 
            p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) 
            || p.Name.Equals(EntityType.Name + "ID", StringComparison.OrdinalIgnoreCase));
        
        ArgumentNullException.ThrowIfNull(key, 
            "Couldn't find the intideficator key property. Check if your entity has a key named \"id\" or property with KeyAttribute");
        return key;
    }
    
    private string GetInternalName()
    {
        var entityType = typeof(TEntity);
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
        return string.IsNullOrEmpty(name)
            ? throw new UnitOfWorkArgumentNullException($"Cannot read type name from entity in ICollectionNameSelector.GetMongoCollectionName. Argument is NULL: {nameof(name)}")
            : name;
    }
}