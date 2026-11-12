namespace OrderSystem.UnitTests.Entities;
using OrderSystem.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldInitializeOrderItemWithProductAndQuantity()
    {
        // Arrange
        var product = new Product("Test Product", 10.99m);
        int quantity = 2;

        // Act
        var orderItem = new OrderItem(product, quantity);

        // Assert
        Assert.Equal(product, orderItem.Product);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(product.CurrentPrice, orderItem.UnitPrice);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenProductIsNull()
    {
        // Arrange
        Product? nullProduct = null;
        int quantity = 2;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new OrderItem(nullProduct!, quantity));
        Assert.Equal("product", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsZeroOrNegative()
    {
        // Arrange
        var product = new Product("Test Product", 10.99m);
        int invalidQuantity = 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(product, invalidQuantity));
        Assert.Equal("quantity", exception.ParamName);
    }

    [Fact]
    public void GetTotalPrice_ShouldReturnCorrectTotal_WhenQuantityAndUnitPriceAreSet()
    {
        // Arrange
        var product = new Product("Test Product", 10.99m);
        var orderItem = new OrderItem(product, 3);

        // Act
        var totalPrice = orderItem.GetTotalPrice();

        // Assert
        Assert.Equal(32.97m, totalPrice);
    }
}