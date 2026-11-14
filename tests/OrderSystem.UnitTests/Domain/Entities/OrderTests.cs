namespace OrderSystem.UnitTests.Domain.Entities;
using OrderSystem.Domain.Entities;

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
    public void ChangeItemQuantity_ShouldUpdateQuantity_WhenProductExistsInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);
        order.AddItem(product, 2);

        // Act
        order.ChangeItemQuantity(product, 5);

        // Assert
        Assert.Single(order.GetItems());
        Assert.Equal(product, order.GetItems()[0].Product);
        Assert.Equal(5, order.GetItems()[0].Quantity);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldThrowInvalidOperationException_WhenProductDoesNotExistInOrder()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var product = new Product("Test Product", 10.99m);
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => order.ChangeItemQuantity(product, 5));
        Assert.Equal("Product not found in the order.", exception.Message);
    }

    [Fact]
    public void ChangeItemQuantity_ShouldThrowArgumentNullException_WhenProductIsNull()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        var order = new Order([], customer);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => order.ChangeItemQuantity(null!, 5));
        Assert.Equal("product", exception.ParamName);
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
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => order.ChangeItemQuantity(product, 0));
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
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => order.ChangeItemQuantity(product, -1));
        Assert.Equal("newQuantity", exception.ParamName);
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