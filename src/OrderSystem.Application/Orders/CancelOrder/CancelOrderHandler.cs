namespace OrderSystem.Application.Orders.CancelOrder;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;
using OrderSystem.Domain.Entities;

public sealed class CancelOrderHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Order> HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(
            command.OrderId,
            cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.CancelOrder();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}