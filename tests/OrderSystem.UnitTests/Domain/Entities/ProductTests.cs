namespace OrderSystem.UnitTests.Domain.Entities;
using OrderSystem.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange
        string name = "Test Product";
        decimal price = 10.99m;

        // Act
        var product = new Product(name, price);

        // Assert
        Assert.Equal(name, product.Name);
        Assert.Equal(price, product.CurrentPrice);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNameIsNullOrWhitespace()
    {
        // Arrange
        string name = "   ";
        string? nullName = null;
        decimal price = 10.99m;

        // Act & Assert

        // whitespace name test
        var exception = Assert.Throws<ArgumentException>(
            () => new Product(name, price));
        Assert.Equal("name", exception.ParamName);

        // null name test
        var nullException = Assert.Throws<ArgumentException>(
            () => new Product(nullName!, price));
        Assert.Equal("name", nullException.ParamName);
    }

    [Fact]
    public void Constructor_ThrowsArgumentOutOfRangeException_WhenPriceIsNegative()
    {
        // Arrange
        string name = "Test Product";
        decimal price = -5.00m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Product(name, price));
        Assert.Equal("currentPrice", exception.ParamName);
    }

    [Fact]
    public void SetPrice_UpdatesCurrentPrice_WhenNewPriceIsValid()
    {
        // Arrange
        var product = new Product("Test Product", 10.99m);
        decimal newPrice = 15.99m;

        // Act
        product.SetPrice(newPrice);

        // Assert
        Assert.Equal(newPrice, product.CurrentPrice);
    }

    [Fact]
    public void SetPrice_ThrowsArgumentOutOfRangeException_WhenNewPriceIsNegative()
    {
        // Arrange
        var product = new Product("Test Product", 10.99m);
        decimal newPrice = -5.00m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => product.SetPrice(newPrice));
        Assert.Equal("newPrice", exception.ParamName);
    }


}
