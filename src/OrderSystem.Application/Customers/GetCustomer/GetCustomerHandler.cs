namespace OrderSystem.Application.Customers.GetCustomer;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class GetCustomerHandler(
    ICustomerRepository customers)
{
    private readonly ICustomerRepository _customers = customers;

    public Task<Customer?> HandleAsync (
        GetCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        return _customers.GetByIdAsync(query.Id, cancellationToken);
    }
}