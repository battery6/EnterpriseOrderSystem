namespace OrderSystem.UnitTests.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Persistence;
using OrderSystem.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepositoryTests
{
    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsCustomer()
    {
        await using var dbContext = CreateDbContext();
        var repository = new CustomerRepository(dbContext);
        var customer = new Customer("Test customer", "customer@example.com");

        await repository.AddAsync(customer);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(customer.Id);

        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Email, result.Email);
    }

    private static OrderSystemDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderSystemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrderSystemDbContext(options);
    }
}
