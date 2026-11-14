namespace OrderSystem.UnitTests.Application.Orders.CreateOrder;

using OrderSystem.Application.Orders.CreateOrder;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_ShouldReturnValid_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            new CreateOrderItemInput(Guid.NewGuid(), 1)
        ]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenCustomerIdIsEmpty()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.Empty,
        [
            new CreateOrderItemInput(Guid.NewGuid(), 1)
        ]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenOrderItemsIsEmpty()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(), []);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OrderItems");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenOrderItemsContainsNull()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            null!
        ]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OrderItems[0]");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenOrderItemHasEmptyProductId()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            new CreateOrderItemInput(Guid.Empty, 1)
        ]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OrderItems[0].ProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldReturnError_WhenOrderItemHasNonPositiveQuantity(int quantity)
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            new CreateOrderItemInput(Guid.NewGuid(), quantity)
        ]);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OrderItems[0].Quantity");
    }
}