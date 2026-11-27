namespace OrderSystem.Domain.Exceptions;

public sealed class OrderItemNotFoundException(Guid orderId, Guid productId)
    : InvalidOperationException(
        $"Order '{orderId}' does not contain item '{productId}'.")
{
    public Guid OrderId { get; } = orderId;
    public Guid ProductId { get; } = productId;
}