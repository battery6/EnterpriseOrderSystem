namespace OrderSystem.Domain.Exceptions;

using OrderSystem.Domain.Enums;

public sealed class OrderNotEditableException(Guid orderId, OrderStatus orderStatus)
    : InvalidOperationException(
        $"Order '{orderId}' with status '{orderStatus}' cannot be modified.")
{
    public Guid OrderId { get; } = orderId;
    public OrderStatus Status { get; } = orderStatus;
}