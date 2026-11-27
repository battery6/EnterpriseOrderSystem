namespace OrderSystem.Application.Orders.AddOrderItem;

public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity);