namespace OrderSystem.Application.Customers.ChangeCustomerEmail;

using FluentValidation;
using OrderSystem.Domain.Entities;

public sealed class ChangeCustomerEmailCommandValidator : AbstractValidator<ChangeCustomerEmailCommand>
{
    public ChangeCustomerEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(Customer.MaxEmailLength)
            .WithMessage($"Email cannot exceed {Customer.MaxEmailLength} characters.");
    }
}