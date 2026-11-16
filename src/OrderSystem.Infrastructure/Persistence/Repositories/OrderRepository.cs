namespace OrderSystem.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public sealed class OrderRepository(OrderSystemDbContext dbContext) : IOrderRepository
{
    private readonly OrderSystemDbContext _dbContext = dbContext;

    public async Task<Order?> GetByIdAsync(
        Guid orderId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
        .Include(order => order.Customer)
        .Include("Items.Product")
        .FirstOrDefaultAsync(
            order => order.Id == orderId, 
            cancellationToken);
    }

    public async Task AddAsync(
        Order order, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }
}