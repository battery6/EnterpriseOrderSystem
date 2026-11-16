namespace OrderSystem.UnitTests.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;
using OrderSystem.Infrastructure.Persistence.Repositories;

public sealed class OrderRepositoryTests
{
    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsOrder()
    {
        await using var dbContext = CreateDbContext();
        var repository = new OrderRepository(dbContext);
        var customer = new Customer("Test customer", "customer@example.com");
        var order = new Order([], customer);

        await dbContext.Customers.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        await repository.AddAsync(order);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    private static OrderSystemDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderSystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrderSystemDbContext(options);
    }
}
