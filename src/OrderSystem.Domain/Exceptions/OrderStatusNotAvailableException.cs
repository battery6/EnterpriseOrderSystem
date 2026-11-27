namespace OrderSystem.Domain.Exceptions;

using OrderSystem.Domain.Enums;

public sealed class OrderStatusNotAvailableException(
    Guid orderId, OrderStatus activeStatus, OrderStatus requestedStatus)
    : InvalidOperationException(
        $"Order '{orderId}' cannot change status from '{activeStatus}' to '{requestedStatus}'.")
{
    public Guid OrderId { get; } = orderId;
    public OrderStatus ActiveStatus { get; } = activeStatus;
    public OrderStatus RequestedStatus { get; } = requestedStatus;
}