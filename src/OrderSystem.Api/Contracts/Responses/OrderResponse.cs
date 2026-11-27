namespace OrderSystem.Api.Contracts.Responses;

using OrderSystem.Domain.Enums;

public sealed record OrderResponse(
    Guid Id, 
    Guid CustomerId, 
    DateTime CreatedAt,
    OrderStatus Status, 
    decimal TotalPrice, 
    IReadOnlyList<OrderItemResponse> Items);