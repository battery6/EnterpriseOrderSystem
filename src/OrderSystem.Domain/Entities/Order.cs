namespace OrderSystem.Domain.Entities;

using OrderSystem.Domain.Common;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Exceptions;

public class Order : BaseEntity
{
    private List<OrderItem> Items { get; set; }
    public Customer Customer { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Placed;
 
    private Order()
    {
        Items = [];
        Customer = null!;
    }

    public Order(List<OrderItem> items, Customer customer)
    {
        Items = items 
            ?? throw new ArgumentNullException(
                nameof(items), "Items cannot be null.");

        Customer = customer 
            ?? throw new ArgumentNullException(
                nameof(customer), "Customer cannot be null.");
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Placed)
        {
            throw new OrderNotEditableException(Id, Status);
        }

        if (product == null)
        {
            throw new ArgumentNullException(
                nameof(product), "Product cannot be null.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be greater than zero.");
        }

        var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existingItem != null)
        {
            var newQuantity = checked(existingItem.Quantity + quantity);
            existingItem.SetQuantity(newQuantity);
        }
        else
        {
            var orderItem = new OrderItem(product, quantity);
            Items.Add(orderItem);
        }

        UpdateTimestamp();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Placed)
        {
            throw new OrderNotEditableException(Id, Status);
        }        

        var item = Items.FirstOrDefault(
            item => item.Product.Id == productId) 
            ?? throw new OrderItemNotFoundException(Id, productId);

        if( Items.Count == 1)
        {
            throw new OrderCannotBeEmptyException();
        }

        Items.Remove(item);
        UpdateTimestamp();
    }

    public void ChangeItemQuantity(Guid productId, int newQuantity)
    {
        if (Status != OrderStatus.Placed)
        {
            throw new OrderNotEditableException(Id, Status);
        }

        if (newQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(newQuantity), "Quantity must be greater than zero.");
        }

        var existingItem = Items.FirstOrDefault(i => i.Product.Id == productId);
        if (existingItem != null)
        {
            existingItem.SetQuantity(newQuantity);
            UpdateTimestamp();
        }
        else
        {
            throw new OrderItemNotFoundException(Id, productId);
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

    public void ProcessOrder()
    {
        if (Status != OrderStatus.Placed)
        {
            throw new OrderStatusNotAvailableException(
                Id, Status, OrderStatus.Processing);
        }

        if(Items.Count == 0)
        {
            throw new OrderCannotBeEmptyException();
        }

        Status = OrderStatus.Processing;
        UpdateTimestamp();
    }

    public void CancelOrder()
    {
        if(Status != OrderStatus.Placed && 
            Status != OrderStatus.Processing)
        {
            throw new OrderStatusNotAvailableException(
                Id, Status, OrderStatus.Cancelled);
        }

        Status = OrderStatus.Cancelled;
        UpdateTimestamp();
    }
}