namespace OrderSystem.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name, 
    decimal CurrentPrice);