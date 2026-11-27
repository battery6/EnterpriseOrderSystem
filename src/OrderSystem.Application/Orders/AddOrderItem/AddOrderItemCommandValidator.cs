namespace OrderSystem.Application.Orders.AddOrderItem;

using FluentValidation;

public sealed class AddOrderItemCommandValidator
    : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator() {

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.");
        
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}