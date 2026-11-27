namespace OrderSystem.Application.Customers.GetCustomer;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class GetCustomerHandler(
    ICustomerRepository customers)
{
    private readonly ICustomerRepository _customers = customers;

    public async Task<Customer?> HandleAsync (
        GetCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customers.GetByIdAsync(
            query.Id, 
            cancellationToken);

        if(customer is null || customer.IsArchived)
        {
            return null;
        }

        return customer;
    }
}