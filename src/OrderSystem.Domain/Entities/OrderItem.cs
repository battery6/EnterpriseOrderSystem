namespace OrderSystem.Domain.Entities;
using OrderSystem.Domain.Common;

public class OrderItem : BaseEntity
{
    public Product Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderItem(Product product, int quantity)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        Product = product;
        Quantity = quantity;
        UnitPrice = product.CurrentPrice;
    }

    public decimal GetTotalPrice()
    {
        return UnitPrice * Quantity;
    }

    internal void SetQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");
        }
        Quantity = newQuantity;
        UpdateTimestamp();
    }
}