namespace OrderSystem.Application.Orders.GetOrder;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class GetOrderHandler(
    IOrderRepository orders)
{
    private readonly IOrderRepository _orders = orders;

    public Task<Order?> HandleAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken = default)
    {
        return _orders.GetByIdAsync(query.Id, cancellationToken);
    }
}