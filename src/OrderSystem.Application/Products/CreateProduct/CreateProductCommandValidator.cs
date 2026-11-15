namespace OrderSystem.Application.Products.CreateProduct;

using FluentValidation;
using OrderSystem.Domain.Entities;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Product.MaxNameLength)
            .WithMessage($"Name cannot exceed {Product.MaxNameLength} characters.");

        RuleFor(x => x.CurrentPrice)
            .GreaterThanOrEqualTo(Product.LeastPrice)
            .WithMessage($"CurrentPrice must be greater than or equal to {Product.LeastPrice}.");
    }
}