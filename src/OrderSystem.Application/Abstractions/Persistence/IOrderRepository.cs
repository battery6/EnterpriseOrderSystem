namespace OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public interface IOrderRepository
{
    Task AddAsync(
        Order order, 
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOrdersForCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOrderItemsForProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}