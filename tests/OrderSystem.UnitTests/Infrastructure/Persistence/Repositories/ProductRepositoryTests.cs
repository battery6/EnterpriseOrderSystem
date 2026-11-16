namespace OrderSystem.UnitTests.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;
using OrderSystem.Infrastructure.Persistence.Repositories;

public sealed class ProductRepositoryTests
{
    [Fact]
    public async Task GetByIdsAsync_ReturnsOnlyRequestedProducts()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ProductRepository(dbContext);
        var requestedProduct = new Product("Requested product", 10m);
        var otherProduct = new Product("Other product", 20m);

        await repository.AddAsync(requestedProduct);
        await repository.AddAsync(otherProduct);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdsAsync([requestedProduct.Id]);

        var product = Assert.Single(result);
        Assert.Equal(requestedProduct.Id, product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenItExists()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ProductRepository(dbContext);
        var product = new Product("Test product", 10m);

        await repository.AddAsync(product);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenProductDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        var repository = new ProductRepository(dbContext);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    private static OrderSystemDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderSystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrderSystemDbContext(options);
    }
}
