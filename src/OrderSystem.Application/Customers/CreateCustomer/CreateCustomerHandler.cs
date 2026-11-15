namespace OrderSystem.Application.Customers.CreateCustomer;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class CreateCustomerHandler(
    IValidator<CreateCustomerCommand> validator,
    ICustomerRepository customers,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<CreateCustomerCommand> _validator = validator;
    private readonly ICustomerRepository _customers = customers;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Customer> HandleAsync(
        CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = new Customer(command.Name, command.Email);
        await _customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer;
    }
}