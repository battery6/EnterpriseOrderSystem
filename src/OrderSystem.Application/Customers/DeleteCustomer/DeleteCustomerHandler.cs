namespace OrderSystem.Application.Customers.DeleteCustomer;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class DeleteCustomerHandler(
    ICustomerRepository customers,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly ICustomerRepository _customers = customers;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(
        DeleteCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customers.GetByIdAsync(
            command.Id,
            cancellationToken)
            ?? throw new CustomerNotFoundException(command.Id);

        var hasOrders = await _orders.HasOrdersForCustomerAsync(
            customer.Id,
            cancellationToken);

        if (hasOrders)
        {
            customer.Archive();
        }
        else{
            _customers.Remove(customer);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}