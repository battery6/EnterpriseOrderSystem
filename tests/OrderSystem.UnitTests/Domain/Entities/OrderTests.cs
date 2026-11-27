namespace OrderSystem.UnitTests.Domain.Entities;

using OrderSystem.Domain.Exceptions;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;

public class OrderTests
{

    [Fact]
    public void Constructor_ShouldInitializeOrderWithCustomerAndEmptyItems()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        // Act
        var order = new Order([], customer);
        // Assert
        Assert.Equal(customer, order.Customer);
        Assert.Empty(order.GetItems());
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCustomerIsNull()
    {
        // Arrange
        Customer? nullCustomer = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Order([], nullCustomer!));
        Assert.Equal("customer", exception.ParamName);
    }

    [Fact]
    public void AddItem_ShouldAddNewItem_WhenProductIsNotInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);

        // Act
        order.AddItem(product, 2);

        // Assert
        Assert.Single(order.GetItems());
        Assert.Equal(product, order.GetItems()[0].Product);
        Assert.Equal(2, order.GetItems()[0].Quantity);
    }

    [Fact]
    public void AddItem_ShouldUpdateQuantity_WhenProductAlreadyExistsInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        order.AddItem(product, 3);

        // Assert
        Assert.Single(order.GetItems());
        Assert.Equal(product, order.GetItems()[0].Product);
        Assert.Equal(5, order.GetItems()[0].Quantity);
    }

    [Fact]
    public void AddItem_ShouldThrowArgumentNullException_WhenProductIsNull()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        Product? nullProduct = null;
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => order.AddItem(nullProduct!, 2));
        Assert.Equal("product", exception.ParamName);
    }

    [Fact]
    public void AddItem_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsZero()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => order.AddItem(product, 0));
        Assert.Equal("quantity", exception.ParamName);
    }

    [Fact]
    public void AddItem_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsNegative()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => order.AddItem(product, -1));
        Assert.Equal("quantity", exception.ParamName);
    }

    [Fact]
    public void AddItem_ShouldThrowOverflowException_WhenCombinedQuantityExceedsIntMax()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10m);
        var existingItem = new OrderItem(product, int.MaxValue);
        var order = new Order([existingItem], customer);

        Assert.Throws<OverflowException>(() => order.AddItem(product, 1));
        Assert.Equal(int.MaxValue, Assert.Single(order.GetItems()).Quantity);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldUpdateQuantity_WhenProductExistsInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        order.ChangeItemQuantity(product.Id, 5);

        // Assert
        Assert.Single(order.GetItems());
        Assert.Equal(product, order.GetItems()[0].Product);
        Assert.Equal(5, order.GetItems()[0].Quantity);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldThrowOrderItemNotFoundException_WhenProductDoesNotExistInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<OrderItemNotFoundException>(
            () => order.ChangeItemQuantity(product.Id, 5));
        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal(product.Id, exception.ProductId);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldThrowArgumentOutOfRangeException_WhenNewQuantityIsZero()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => order.ChangeItemQuantity(product.Id, 0));
        Assert.Equal("newQuantity", exception.ParamName);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldThrowArgumentOutOfRangeException_WhenNewQuantityIsNegative()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => order.ChangeItemQuantity(product.Id, -1));
        Assert.Equal("newQuantity", exception.ParamName);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveMatchingProductLine()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var firstProduct = new Product("First Product", 10m);
        var secondProduct = new Product("Second Product", 20m);
        var order = new Order([], customer);
        order.AddItem(firstProduct, 1);
        order.AddItem(secondProduct, 2);

        order.RemoveItem(firstProduct.Id);

        var remainingItem = Assert.Single(order.GetItems());
        Assert.Equal(secondProduct.Id, remainingItem.Product.Id);
        Assert.Equal(2, remainingItem.Quantity);
    }

    [Fact]
    public void RemoveItem_ShouldThrowOrderCannotBeEmpty_WhenRemovingLastLine()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10m);
        var order = new Order([], customer);
        order.AddItem(product, 1);

        Assert.Throws<OrderCannotBeEmptyException>(
            () => order.RemoveItem(product.Id));

        Assert.Single(order.GetItems());
    }

    [Fact]
    public void RemoveItem_ShouldThrowOrderItemNotFound_WhenProductIsNotOnOrder()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10m);
        var order = new Order([], customer);

        var exception = Assert.Throws<OrderItemNotFoundException>(
            () => order.RemoveItem(product.Id));

        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal(product.Id, exception.ProductId);
    }

    [Fact]
    public void CancelOrder_ShouldSetCancelledStatus()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);

        order.CancelOrder();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void CancelOrder_ShouldRejectAlreadyCancelledOrder()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);
        order.CancelOrder();

        var exception = Assert.Throws<OrderStatusNotAvailableException>(
            order.CancelOrder);

        Assert.Equal(OrderStatus.Cancelled, exception.ActiveStatus);
        Assert.Equal(OrderStatus.Cancelled, exception.RequestedStatus);
    }

    [Fact]
    public void ProcessOrder_ShouldSetProcessingStatus_WhenOrderHasItems()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);
        order.AddItem(new Product("Test Product", 10m), 1);

        order.ProcessOrder();

        Assert.Equal(OrderStatus.Processing, order.Status);
    }

    [Fact]
    public void ProcessOrder_ShouldRejectEmptyOrder()
    {
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);

        Assert.Throws<OrderCannotBeEmptyException>(order.ProcessOrder);
        Assert.Equal(OrderStatus.Placed, order.Status);
    }

    [Fact]
    public void GetItems_ShouldReturnAllOrderItems()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        var items = order.GetItems();

        // Assert
        Assert.Single(items);
        Assert.Equal(product, items[0].Product);
        Assert.Equal(2, items[0].Quantity);
    }

    [Fact]
    public void GetItems_ShouldReturnEmptyList_WhenNoItemsAdded()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);

        // Act
        var items = order.GetItems();

        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public void GetItems_ShouldNotAllowModificationOfReturnedList()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        var items = order.GetItems();
        var list = Assert.IsAssignableFrom<IList<OrderItem>>(items);
        var anotherItem = new OrderItem(new Product("Another Product", 15.99m), 1);

        // Assert
        Assert.Throws<NotSupportedException>(() => list.Add(anotherItem));
        Assert.Single(order.GetItems());
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnCorrectTotal_WhenItemsAreAdded()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        var totalPrice = order.GetTotalPrice();

        // Assert
        Assert.Equal(21.98m, totalPrice);
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnZero_WhenNoItemsAdded()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);

        // Act
        var totalPrice = order.GetTotalPrice();

        // Assert
        Assert.Equal(0m, totalPrice);
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnCorrectTotal_WhenMultipleItemsAreAdded()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product1 = new Product("Test Product 1", 10.99m);
        var product2 = new Product("Test Product 2", 20.99m);
        var order = new Order([], customer);
        order.AddItem(product1, 2);
        order.AddItem(product2, 1);

        // Act
        var totalPrice = order.GetTotalPrice();

        // Assert
        Assert.Equal(42.97m, totalPrice);
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnCorrectTotal_WhenItemQuantityIsChanged()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        var totalPrice = order.GetTotalPrice();

        // Assert
        Assert.Equal(21.98m, totalPrice);
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnCorrectTotal_WhenItemIsAddedAgain()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);
        order.AddItem(product, 1);

        // Act
        var totalPrice = order.GetTotalPrice();

        // Assert
        Assert.Equal(32.97m, totalPrice);
    }

}