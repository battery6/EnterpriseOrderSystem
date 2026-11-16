namespace OrderSystem.Application.Products.GetProduct;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class GetProductHandler(
    IProductRepository products)
{
    private readonly IProductRepository _products = products;

    public Task<Product?> HandleAsync(
        GetProductQuery query,
        CancellationToken cancellationToken = default)
    {
        return _products.GetByIdAsync(query.Id, cancellationToken);
    }
}