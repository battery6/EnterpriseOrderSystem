namespace OrderSystem.Domain.Exceptions;

public sealed class CustomerArchivedException(Guid customerId)
    : InvalidOperationException($"Customer '{customerId}' is archived.")
{
    public Guid CustomerId { get; } = customerId;
}