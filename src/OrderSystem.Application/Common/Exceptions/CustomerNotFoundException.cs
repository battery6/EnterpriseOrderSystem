namespace OrderSystem.Application.Common.Exceptions;

public sealed class CustomerNotFoundException(Guid customerId) 
    : Exception($"Customer with ID '{customerId}' was not found.")
{
    public Guid CustomerId { get; } = customerId;
}