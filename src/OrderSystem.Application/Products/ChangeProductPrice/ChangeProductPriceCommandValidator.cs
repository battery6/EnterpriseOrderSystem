namespace OrderSystem.Application.Products.ChangeProductPrice;

using FluentValidation;
using OrderSystem.Domain.Entities;

public sealed class ChangeProductPriceCommandValidator : AbstractValidator<ChangeProductPriceCommand>
{
    public ChangeProductPriceCommandValidator()
    {
        RuleFor(x => x.CurrentPrice)
            .GreaterThanOrEqualTo(Product.LeastPrice)
            .WithMessage($"CurrentPrice must be greater than or equal to {Product.LeastPrice}.");
    }
}