using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Core.DbConverters;

public class EntityToJsonConverter<TEntity> : ValueConverter<TEntity, string>
{
    public EntityToJsonConverter()
        : base(Serialize(), Deserialize(), null)
    {
    }

    private static Expression<Func<TEntity, string>> Serialize()
        => to => JsonSerializer.Serialize(to, JsonSerializerOptions.Default);

    private static Expression<Func<string, TEntity>> Deserialize()
        => from => JsonSerializer.Deserialize<TEntity>(from, JsonSerializerOptions.Default)!;
}