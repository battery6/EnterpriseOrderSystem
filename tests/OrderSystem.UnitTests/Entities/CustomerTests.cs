namespace OrderSystem.UnitTests.Entities;
using OrderSystem.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldInitializeCustomerWithNameAndEmail()
    {
        // Arrange
        string name = "John Doe";
        string email = "john.doe@example.com";

        // Act
        var customer = new Customer(name, email);

        // Assert
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenNameIsNull()
    {
        // Arrange
        string? nullName = null;
        string email = "john.doe@example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Customer(nullName!, email));
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        string emptyName = "";
        string email = "john.doe@example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Customer(emptyName, email));
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenEmailIsNull()
    {
        // Arrange
        string name = "John Doe";
        string? nullEmail = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Customer(name, nullEmail!));
        Assert.Equal("email", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenEmailIsInvalid()
    {
        // Arrange
        string name = "John Doe";
        string invalidEmail = "invalid-email";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Customer(name, invalidEmail));
        Assert.Equal("email", exception.ParamName);
    }

    [Fact]
    public void UpdateEmail_ShouldUpdateEmail_WhenNewEmailIsValid()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        string newEmail = "john.new@example.com";

        // Act
        customer.UpdateEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, customer.Email);
    }

    [Fact]
    public void UpdateEmail_ShouldThrowArgumentException_WhenNewEmailIsInvalid()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        string invalidEmail = "invalid-email";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => customer.UpdateEmail(invalidEmail));
        Assert.Equal("newEmail", exception.ParamName);
    }

    [Fact]
    public void UpdateEmail_ShouldThrowArgumentNullException_WhenNewEmailIsNull()
    {
        // Arrange
        var customer = new Customer("John Doe", "john.doe@example.com");
        string? nullEmail = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => customer.UpdateEmail(nullEmail!));
        Assert.Equal("newEmail", exception.ParamName);
    }
}