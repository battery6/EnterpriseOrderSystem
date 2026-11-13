namespace OrderSystem.Domain.Entities;
using OrderSystem.Domain.Common;

public class Order : BaseEntity
{
    private List<OrderItem> Items { get; set; }
    public Customer Customer { get; private set; }

    public Order(List<OrderItem> items, Customer customer)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items), "Items cannot be null.");
        Customer = customer ?? throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
    }

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.SetQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(product, quantity);
            Items.Add(orderItem);
        }

        UpdateTimestamp();
    }

    public void ChangeItemQuantity(Product product, int newQuantity)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        if (newQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");
        }

        var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.SetQuantity(newQuantity);
            UpdateTimestamp();
        }
        else
        {
            throw new InvalidOperationException("Product not found in the order.");
        }
    }

    public IReadOnlyList<OrderItem> GetItems()
    {
        return Items.AsReadOnly();
    }

    public decimal GetTotalPrice()
    {
        return Items.Sum(item => item.GetTotalPrice());
    }
}