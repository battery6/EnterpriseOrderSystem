namespace OrderSystem.Application.Common.Exceptions;

public sealed class ProductsNotFoundException(IReadOnlyList<Guid> productIds) 
    : Exception($"Products with IDs '[{string.Join(", ", productIds)}]' were not found.")
{
    public Guid[] ProductIds { get; } = [.. productIds];
}