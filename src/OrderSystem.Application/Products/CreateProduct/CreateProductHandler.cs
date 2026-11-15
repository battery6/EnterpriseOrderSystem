namespace OrderSystem.Application.Products.CreateProduct;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class CreateProductHandler (
    IValidator<CreateProductCommand> validator,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{

    private readonly IValidator<CreateProductCommand> validator = validator;
    private readonly IProductRepository productRepository = productRepository;
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<Product> HandleAsync(
        CreateProductCommand command, 
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var product = new Product(command.Name, command.CurrentPrice);

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product;
    }
}