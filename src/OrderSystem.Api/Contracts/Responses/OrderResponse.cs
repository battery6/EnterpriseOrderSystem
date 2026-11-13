namespace OrderSystem.Api.Contracts.Responses;

public sealed record OrderResponse(
    Guid Id, Guid CustomerId, DateTime CreatedAt, decimal TotalPrice, IReadOnlyList<OrderItemResponse> Items);