namespace OrderSystem.Domain.Exceptions;

public sealed class ProductArchivedException(Guid productId)
    : InvalidOperationException($"Product '{productId}' is archived.")
{
    public Guid ProductId { get; } = productId;
}