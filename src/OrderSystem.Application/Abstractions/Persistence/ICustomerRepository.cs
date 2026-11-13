namespace OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(
        Guid customerId, 
        CancellationToken cancellationToken = default);
}