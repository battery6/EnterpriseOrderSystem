namespace OrderSystem.Application.Products.ChangeProductPrice;

public sealed record ChangeProductPriceCommand(
    Guid ProductId, 
    decimal CurrentPrice);