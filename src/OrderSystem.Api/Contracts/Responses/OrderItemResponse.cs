namespace OrderSystem.Api.Contracts.Responses;

public sealed record OrderItemResponse(
    Guid ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);