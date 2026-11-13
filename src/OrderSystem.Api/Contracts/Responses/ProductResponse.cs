namespace OrderSystem.Api.Contracts.Responses;

public sealed record ProductResponse(
    Guid Id, string Name, decimal CurrentPrice);