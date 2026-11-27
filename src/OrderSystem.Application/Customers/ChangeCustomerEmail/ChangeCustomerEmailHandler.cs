namespace OrderSystem.Application.Customers.ChangeCustomerEmail;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class ChangeCustomerEmailHandler(
    IValidator<ChangeCustomerEmailCommand> validator,
    ICustomerRepository customers,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<ChangeCustomerEmailCommand> _validator = validator;
    private readonly ICustomerRepository _customers = customers;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Customer> HandleAsync(
        ChangeCustomerEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customers.GetByIdAsync(
            command.Id,
            cancellationToken) 
        ?? throw new CustomerNotFoundException(command.Id);

        customer.UpdateEmail(command.Email);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer;
    }
}