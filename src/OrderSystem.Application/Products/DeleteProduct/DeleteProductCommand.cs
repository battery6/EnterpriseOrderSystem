namespace OrderSystem.Application.Products.DeleteProduct;

public sealed record DeleteProductCommand(
    Guid ProductId
);