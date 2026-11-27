namespace OrderSystem.Application.Products.GetProduct;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;

public sealed class GetProductHandler(
    IProductRepository products)
{
    private readonly IProductRepository _products = products;

    public async Task<Product?> HandleAsync(
        GetProductQuery query,
        CancellationToken cancellationToken = default)
    {
        
        var product = await _products.GetByIdAsync(query.ProductId, cancellationToken);
    
        if(product is null || product.IsArchived)
            {
                return null;
            }
        
        return product;
    }
}