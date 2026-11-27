namespace OrderSystem.Application.Products.ChangeProductPrice;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class ChangeProductPriceHandler(
    IValidator<ChangeProductPriceCommand> validator,
    IProductRepository Products,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<ChangeProductPriceCommand> _validator = validator;
    private readonly IProductRepository _Products = Products;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Product> HandleAsync(
        ChangeProductPriceCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var Product = await _Products.GetByIdAsync(
            command.ProductId,
            cancellationToken) 
        ?? throw new ProductNotFoundException(command.ProductId);

        Product.UpdatePrice(command.CurrentPrice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Product;
    }
}