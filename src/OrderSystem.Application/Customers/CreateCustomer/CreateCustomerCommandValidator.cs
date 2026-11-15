namespace OrderSystem.Application.Customers.CreateCustomer;

using FluentValidation;
using OrderSystem.Domain.Entities;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(Customer.MaxNameLength)
            .WithMessage($"Name cannot exceed {Customer.MaxNameLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(Customer.MaxEmailLength)
            .WithMessage($"Email cannot exceed {Customer.MaxEmailLength} characters.");
    }
}