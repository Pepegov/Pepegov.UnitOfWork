using Pepegov.UnitOfWork.EntityFramework.Test.Core;
using Pepegov.UnitOfWork.EntityFramework.Test.Entity;
using Pepegov.UnitOfWork.EntityFramework.Test.Fakes;

namespace Pepegov.UnitOfWork.EntityFramework.Test.Test;

public class RepositoryQuerybleExpressionTests : TestBase
{
    #region Min

    [Test]
    public void Min_ShouldReturnMin_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act
        var personMinAge = personRepository.Min(s => s.Age);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Min(x => x.Age)));
    }
    
    [Test]
    public void Min_ShouldReturnMin_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        SeedData(persons.ToArray());
        
        // Act
        var personMinAge = personRepository.Min(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Where(w => w.Age > 10).Min(x => x.Age)));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnMin_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMinAge = await personRepository.MinAsync(s => s.Age);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Min(x => x.Age)));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnMin_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMinAge = await personRepository.MinAsync(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(persons.Where(w => w.Age > 10).Min(x => x.Age)));
    }

    [Test]
    public void Min_ShouldReturnDefault_WhenNoDataAndDefaultIfEmptyTrue_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act
        var personMinAge = personRepository.Min(
            selector: s => s.Age,
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public void Min_ShouldThrowException_WhenNoDataAndDefaultIfEmptyFalse_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            personRepository.Min(
                selector: s => s.Age,
                defaultIfEmpty: false));
    }
    
    [Test]
    public void Min_ShouldReturnDefault_WhenNoDataWithPredicateAndDefaultIfEmptyTrue_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act
        var personMinAge = personRepository.Min(
            selector: s => s.Age,
            predicate: p => p.Age > 1000, // Predicate that won't find any data
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public void Min_ShouldThrowException_WhenNoDataWithPredicateAndDefaultIfEmptyFalse_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            personRepository.Min(
                selector: s => s.Age,
                predicate: p => p.Age > 1000, // Predicate that won't find any data
                defaultIfEmpty: false));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnDefault_WhenNoDataAndDefaultIfEmptyTrue_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act
        var personMinAge = await personRepository.MinAsync(
            selector: s => s.Age,
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public async Task MinAsync_ShouldThrowException_WhenNoDataAndDefaultIfEmptyFalse_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await personRepository.MinAsync(
                selector: s => s.Age,
                defaultIfEmpty: false));
    }
    
    [Test]
    public async Task MinAsync_ShouldReturnDefault_WhenNoDataWithPredicateAndDefaultIfEmptyTrue_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMinAge = await personRepository.MinAsync(
            selector: s => s.Age,
            predicate: p => p.Age > 1000, // Predicate that won't find any data
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMinAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public async Task MinAsync_ShouldThrowException_WhenNoDataWithPredicateAndDefaultIfEmptyFalse_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await personRepository.MinAsync(
                selector: s => s.Age,
                predicate: p => p.Age > 1000, // Predicate that won't find any data
                defaultIfEmpty: false));
    }

    #endregion

    #region Max

    [Test]
    public void Max_ShouldReturnMax_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act
        var personMaxAge = personRepository.Max(s => s.Age);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(persons.Max(x => x.Age)));
    }
    
    [Test]
    public void Max_ShouldReturnMax_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        SeedData(persons.ToArray());
        
        // Act
        var personMaxAge = personRepository.Max(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(persons.Where(w => w.Age > 10).Max(x => x.Age)));
    }
    
    [Test]
    public async Task MaxAsync_ShouldReturnMax_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMaxAge = await personRepository.MaxAsync(s => s.Age);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(persons.Max(x => x.Age)));
    }
    
    [Test]
    public async Task MaxAsync_ShouldReturnMax_IfValueMoreThen10Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMaxAge = await personRepository.MaxAsync(
            selector: s => s.Age,
            predicate: p => p.Age > 10);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(persons.Where(w => w.Age > 10).Max(x => x.Age)));
    }

    [Test]
    public void Max_ShouldReturnDefault_WhenNoDataAndDefaultIfEmptyTrueTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act
        var personMaxAge = personRepository.Max(
            selector: s => s.Age,
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public void Max_ShouldThrowException_WhenNoDataAndDefaultIfEmptyFalseTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            personRepository.Max(
                selector: s => s.Age,
                defaultIfEmpty: false));
    }
    
    [Test]
    public void Max_ShouldReturnDefault_WhenNoDataWithPredicateAndDefaultIfEmptyTrueTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act
        var personMaxAge = personRepository.Max(
            selector: s => s.Age,
            predicate: p => p.Age > 1000, // Predicate that won't find any data
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public void Max_ShouldThrowException_WhenNoDataWithPredicateAndDefaultIfEmptyFalseTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        SeedData(persons.ToArray());
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            personRepository.Max(
                selector: s => s.Age,
                predicate: p => p.Age > 1000, // Predicate that won't find any data
                defaultIfEmpty: false));
    }
    
    [Test]
    public async Task MaxAsync_ShouldReturnDefault_WhenNoDataAndDefaultIfEmptyTrueTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act
        var personMaxAge = await personRepository.MaxAsync(
            selector: s => s.Age,
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public async Task MaxAsync_ShouldThrowException_WhenNoDataAndDefaultIfEmptyFalseTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        // Don't add data to database
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await personRepository.MaxAsync(
                selector: s => s.Age,
                defaultIfEmpty: false));
    }
    
    [Test]
    public async Task MaxAsync_ShouldReturnDefault_WhenNoDataWithPredicateAndDefaultIfEmptyTrueTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var personMaxAge = await personRepository.MaxAsync(
            selector: s => s.Age,
            predicate: p => p.Age > 1000, // Predicate that won't find any data
            defaultIfEmpty: true);

        // Assert
        Assert.That(personMaxAge, Is.EqualTo(0)); // Default value for int
    }
    
    [Test]
    public async Task MaxAsync_ShouldThrowException_WhenNoDataWithPredicateAndDefaultIfEmptyFalseTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(3);
        await SeedDataAsync(persons.ToArray());
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await personRepository.MaxAsync(
                selector: s => s.Age,
                predicate: p => p.Age > 1000, // Predicate that won't find any data
                defaultIfEmpty: false));
    }

    #endregion

    #region Average

    [Test]
    public void Average_ShouldReturnAverage_Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        SeedData(products.ToArray());
        
        // Act
        var productAveragePrice = productRepository.Average(s => s.Price);

        // Assert
        Assert.That(productAveragePrice, Is.EqualTo(products.Average(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public void Average_ShouldReturnAverage_IfValueMoreThen500Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(501, 1000))
            .Generate());
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 500))
            .Generate());
        SeedData(products.ToArray());
        
        // Act
        var productAveragePrice = productRepository.Average(
            selector: s => s.Price,
            predicate: p => p.Price > 500);

        // Assert
        Assert.That(productAveragePrice, Is.EqualTo(products.Where(w => w.Price > 500).Average(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public async Task AverageAsync_ShouldReturnAverage_Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        await SeedDataAsync(products.ToArray());
        
        // Act
        var productAveragePrice = await productRepository.AverageAsync(s => s.Price);

        // Assert
        Assert.That(productAveragePrice, Is.EqualTo(products.Average(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public async Task AverageAsync_ShouldReturnAverage_IfValueMoreThen500Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(501, 1000))
            .Generate());
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 500))
            .Generate());
        await SeedDataAsync(products.ToArray());
        
        // Act
        var productAveragePrice = await productRepository.AverageAsync(
            selector: s => s.Price,
            predicate: p => p.Price > 500);

        // Assert
        Assert.That(productAveragePrice, Is.EqualTo(products.Where(w => w.Price > 500).Average(x => x.Price)).Within(0.01m));
    }

    #endregion

    #region Sum

    [Test]
    public void Sum_ShouldReturnSum_Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        SeedData(products.ToArray());
        
        // Act
        var productSumPrice = productRepository.Sum(s => s.Price);

        // Assert
        Assert.That(productSumPrice, Is.EqualTo(products.Sum(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public void Sum_ShouldReturnSum_IfValueMoreThen500Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(501, 1000))
            .Generate());
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 500))
            .Generate());
        SeedData(products.ToArray());
        
        // Act
        var productSumPrice = productRepository.Sum(
            selector: s => s.Price,
            predicate: p => p.Price > 500);

        // Assert
        Assert.That(productSumPrice, Is.EqualTo(products.Where(w => w.Price > 500).Sum(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public async Task SumAsync_ShouldReturnSum_Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        await SeedDataAsync(products.ToArray());
        
        // Act
        var productSumPrice = await productRepository.SumAsync(s => s.Price);

        // Assert
        Assert.That(productSumPrice, Is.EqualTo(products.Sum(x => x.Price)).Within(0.01m));
    }
    
    [Test]
    public async Task SumAsync_ShouldReturnSum_IfValueMoreThen500Test()
    {
        // Arrange
        var productRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Product>();
        var products = Fake.ProductFaker.CreateProductFaker().Generate(3);
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(501, 1000))
            .Generate());
        products.Add(Fake.ProductFaker.CreateProductFaker()
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 500))
            .Generate());
        await SeedDataAsync(products.ToArray());
        
        // Act
        var productSumPrice = await productRepository.SumAsync(
            selector: s => s.Price,
            predicate: p => p.Price > 500);

        // Assert
        Assert.That(productSumPrice, Is.EqualTo(products.Where(w => w.Price > 500).Sum(x => x.Price)).Within(0.01m));
    }

    #endregion

    #region Take

    [Test]
    public void Take_ShouldReturnFirstTwo_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        SeedData(persons.ToArray());
        
        // Act
        var result = personRepository.Take(2);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<Person>());
        Assert.That(result.Select(x => x.Id), Is.SubsetOf(persons.Select(x => x.Id)));
    }
    
    [Test]
    public void Take_ShouldReturnFirstTwo_WithPredicateTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        SeedData(persons.ToArray());
        
        // Act
        var result = personRepository.Take(2, predicate: p => p.Age > 10);

        // Assert
        Assert.That(result.Count, Is.LessThanOrEqualTo(2));
        Assert.That(result.All(p => p.Age > 10), Is.True);
        Assert.That(result.Select(i => i.Id), Is.SubsetOf(persons.Select(i => i.Id)));
    }
    
    [Test]
    public void Take_ShouldReturnFirstTwo_WithSkipTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        SeedData(persons.ToArray());
        
        // Act
        var result = personRepository.Take(2, skip: 1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<Person>());
        Assert.That(result.Select(i => i.Id), Is.SubsetOf(persons.Select(s => s.Id)));
    }
    
    [Test]
    public void Take_ShouldReturnFirstTwo_WithOrderByTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        SeedData(persons.ToArray());
        
        // Act
        var result = personRepository.Take(2, orderBy: q => q.OrderBy(p => p.Age));

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.Ordered.By("Age"));
        Assert.That(result.Select(i => i.Id), Is.SubsetOf(persons.Select(s => s.Id)));
        
        // Check that the result contains the first 2 items from the sorted list
        var expectedOrdered = persons.OrderBy(p => p.Age).Take(2).ToList();
        Assert.That(result.Select(p => p.Id), Is.EqualTo(expectedOrdered.Select(p => p.Id)));
    }
    
    [Test]
    public void Take_ShouldReturnNames_WithSelectorTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        SeedData(persons.ToArray());
        
        // Act
        var result = personRepository.Take<string>(2, selector: p => p.Name);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<string>());
        Assert.That(result, Is.All.Not.Null);
        Assert.That(result, Is.SubsetOf(persons.Select(p => p.Name)));
    }
    
    [Test]
    public async Task TakeAsync_ShouldReturnFirstTwo_Test()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var result = await personRepository.TakeAsync(2);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<Person>());
        Assert.That(result.Select(s => s.Id), Is.SubsetOf(persons.Select(s => s.Id)));
    }
    
    [Test]
    public async Task TakeAsync_ShouldReturnFirstTwo_WithPredicateTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(11, 100))
            .Generate());
        persons.Add(Fake.PersonFaker.CreatePersonFaker()
            .RuleFor(x => x.Age, f => f.Random.Int(0, 10))
            .Generate());
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var result = await personRepository.TakeAsync(2, predicate: p => p.Age > 10);

        // Assert
        Assert.That(result.Count, Is.LessThanOrEqualTo(2));
        Assert.That(result.All(p => p.Age > 10), Is.True);
        Assert.That(result.Select(s => s.Id), Is.SubsetOf(persons.Select(s => s.Id)));
    }
    
    [Test]
    public async Task TakeAsync_ShouldReturnFirstTwo_WithSkipTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var result = await personRepository.TakeAsync(2, skip: 1);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<Person>());
        Assert.That(result.Select(s => s.Id), Is.SubsetOf(persons.Select(s => s.Id)));
    }
    
    [Test]
    public async Task TakeAsync_ShouldReturnFirstTwo_WithOrderByTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var result = await personRepository.TakeAsync(2, orderBy: q => q.OrderBy(p => p.Age));

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.Ordered.By("Age"));
        Assert.That(result.Select(x => x.Id), Is.SubsetOf(persons.Select(x => x.Id)));
        
        // Check that the result contains the first 2 items from the sorted list
        var expectedOrdered = persons.OrderBy(p => p.Age).Take(2).ToList();
        Assert.That(result.Select(p => p.Id), Is.EqualTo(expectedOrdered.Select(p => p.Id)));
    }
    
    [Test]
    public async Task TakeAsync_ShouldReturnNames_WithSelectorTest()
    {
        // Arrange
        var personRepository = UnitOfWorkEntityFrameworkInstance.GetRepository<Person>();
        var persons = Fake.PersonFaker.CreatePersonFaker().Generate(5);
        await SeedDataAsync(persons.ToArray());
        
        // Act
        var result = await personRepository.TakeAsync<string>(2, selector: p => p.Name);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Is.All.TypeOf<string>());
        Assert.That(result, Is.All.Not.Null);
        Assert.That(result, Is.SubsetOf(persons.Select(p => p.Name)));
    }

    #endregion
}