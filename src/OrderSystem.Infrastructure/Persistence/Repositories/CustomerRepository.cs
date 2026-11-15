namespace OrderSystem.Infrastructure.Persistence.Repositories;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public sealed class CustomerRepository(OrderSystemDbContext dbContext) : ICustomerRepository
{
    private readonly OrderSystemDbContext _dbContext = dbContext;

    public async Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.FindAsync([customerId], cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }
}