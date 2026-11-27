namespace OrderSystem.Application.Orders.RemoveOrderItem;

using FluentValidation;

public sealed class RemoveOrderItemCommandValidator
    : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator() {

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.");
        
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");
    }
}