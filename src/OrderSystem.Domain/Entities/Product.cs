namespace OrderSystem.Domain.Entities;
using OrderSystem.Domain.Common;

// Product class represents a product in the order system with its unique identifier, name, and current price.
public class Product : BaseEntity  
{

    public string Name { get; private set; }
    public decimal CurrentPrice { get; private set; }

    // Constructor to initialize a new product with generated id, given name, and given price.
    public Product(string name, decimal currentPrice)
    {

        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if(currentPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(currentPrice), "Price cannot be negative.");
        }

        Name = name;
        CurrentPrice = currentPrice;
    }

    public void SetPrice(decimal newPrice)
    {
        if(newPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newPrice), "Price cannot be negative.");
        }
        CurrentPrice = newPrice;
        UpdateTimestamp();
    }
}