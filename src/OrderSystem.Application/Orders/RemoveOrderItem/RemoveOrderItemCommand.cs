namespace OrderSystem.Application.Orders.RemoveOrderItem;

public sealed record RemoveOrderItemCommand(
    Guid OrderId,
    Guid ProductId);