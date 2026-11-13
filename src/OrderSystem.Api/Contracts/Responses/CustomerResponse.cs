namespace OrderSystem.Api.Contracts.Responses;

public sealed record CustomerResponse(
    Guid Id, string Name, string Email);