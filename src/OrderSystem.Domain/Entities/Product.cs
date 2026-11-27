namespace OrderSystem.Domain.Entities;

using OrderSystem.Domain.Common;
using OrderSystem.Domain.Exceptions;

// Product class represents a product in the order system with its unique identifier, name, and current price.
public class Product : BaseEntity  
{

    public string Name { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public DateTime? ArchivedAt { get; private set; }

    public const int MaxNameLength = 100;
    public const int LeastPrice = 0;
    public bool IsArchived => ArchivedAt.HasValue;


    // Constructor to initialize a new product with generated id, given name, and given price.
    public Product(string name, decimal currentPrice)
    {

        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if(name.Length > MaxNameLength)
        {
            throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters.", nameof(name));
        }

        if(currentPrice < LeastPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(currentPrice), $"Price cannot be less than {LeastPrice}.");
        }

        Name = name;
        CurrentPrice = currentPrice;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (IsArchived)
        {
            throw new ProductArchivedException(Id);
        }

        if(newPrice < LeastPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(newPrice), $"Price cannot be less than {LeastPrice}.");
        }
        CurrentPrice = newPrice;
        UpdateTimestamp();
    }

    public void Archive()
    {
        if (IsArchived)
        {
            return;
        }

        ArchivedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}