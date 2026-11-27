namespace OrderSystem.Application.Products.DeleteProduct;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class DeleteProductHandler(
    IProductRepository products,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IProductRepository _products = products;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task HandleAsync(
        DeleteProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(
            command.ProductId,
            cancellationToken)
            ?? throw new ProductNotFoundException(command.ProductId);

        var hasOrderItems = await _orders.HasOrderItemsForProductAsync(
            product.Id,
            cancellationToken);

        if (hasOrderItems)
        {
            product.Archive();
        }
        else{
            _products.Remove(product);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}