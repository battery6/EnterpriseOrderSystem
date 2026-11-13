namespace OrderSystem.Application.Orders.CreateOrder;
using FluentValidation;

public sealed class CreateOrderCommandValidator 
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.");

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("OrderItems cannot be empty.");

        RuleForEach(x => x.OrderItems).NotNull()
            .WithMessage("OrderItems cannot contain null values.");

        RuleForEach(x => x.OrderItems)
            .ChildRules(orderItem =>
            {
                orderItem.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("ProductId is required.");

                orderItem.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0.");
            });
    }
}