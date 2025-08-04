using Bogus;
using Product = Pepegov.UnitOfWork.EntityFramework.Test.Entity.Product;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Fakes;

public static partial class Fake
{
    public static class ProductFaker
    {
        public static Faker<Product> CreateProductFaker()
        {
            return new Faker<Product>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(x => x.Quantity, f => f.Random.Int(1, 100));
        }
    }
} 