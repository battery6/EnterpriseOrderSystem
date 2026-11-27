namespace OrderSystem.Application.Common.Exceptions;

public sealed class ProductNotFoundException(Guid productId) 
    : Exception($"Product with ID '{productId}' was not found.")
{
    public Guid ProductId { get; } = productId;
}