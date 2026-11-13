namespace OrderSystem.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId, 
    IReadOnlyList<CreateOrderItemInput> OrderItems);

public sealed record CreateOrderItemInput(
    Guid ProductId, 
    int Quantity);