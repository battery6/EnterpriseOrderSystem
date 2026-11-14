namespace OrderSystem.UnitTests.Application.Orders.CreateOrder;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Orders.CreateOrder;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Common.Exceptions;

public class CreateOrderHandlerTests
{
    // Create fake repositories methods to simulate the behavior of the actual repositories
        private sealed class FakeCustomerRepository(Customer customer) : ICustomerRepository
    {
        public Task<Customer?> GetByIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(customer.Id == customerId ? customer : null);
        }
    }

    private sealed class FakeProductRepository(params Product[] products) : IProductRepository
    {
        public Task<IReadOnlyList<Product>> GetByIdsAsync(
            Guid[] productIds,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Product> matchingProducts = products
                .Where(product => productIds.Contains(product.Id))
                .ToArray();

            return Task.FromResult(matchingProducts);
        }
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public Order? SavedOrder { get; private set; }

        public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            SavedOrder = order;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateAndSaveOrder_WhenCommandIsValid()
    {
        // Arrange
        var customer = new Customer("Test customer", "customer@example.com");
        var product1 = new Product("Product 1", 10m);
        var product2 = new Product("Product 2", 20m);
        var customerRepository = new FakeCustomerRepository(customer);
        var productRepository = new FakeProductRepository(product1, product2);
        var orderRepository = new FakeOrderRepository();

        var command = new CreateOrderCommand(
            customer.Id,
            [
                new CreateOrderItemInput(product1.Id, 2),
                new CreateOrderItemInput(product2.Id, 1)
            ]);

        var handler = new CreateOrderHandler(
            new CreateOrderCommandValidator(),
            customerRepository,
            productRepository,
            orderRepository);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Same(customer, result.Customer);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(2, result.GetItems().Count);
        Assert.Equal(
            [(product1.Id, 2), (product2.Id, 1)],
            result.GetItems().Select(item => (item.Product.Id, item.Quantity)));
        Assert.Same(result, orderRepository.SavedOrder);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowCustomerNotFoundException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var product = new Product("Product 1", 10m);
        var customerRepository = new FakeCustomerRepository(new Customer("Test customer", "customer@example.com"));
        var productRepository = new FakeProductRepository(product);
        var orderRepository = new FakeOrderRepository();

        var command = new CreateOrderCommand(
            Guid.NewGuid(), // Non-existent customer ID
            [
                new CreateOrderItemInput(product.Id, 2)
            ]);

        var handler = new CreateOrderHandler(
            new CreateOrderCommandValidator(),
            customerRepository,
            productRepository,
            orderRepository);

        // Act & Assert
        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => handler.HandleAsync(command, CancellationToken.None));

        Assert.Null(orderRepository.SavedOrder);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowProductsNotFoundException_WhenSomeProductsDoNotExist()
    {
        // Arrange
        var customer = new Customer("Test customer", "customer@example.com");
        var product1 = new Product("Product 1", 10m);
        var customerRepository = new FakeCustomerRepository(customer);
        var productRepository = new FakeProductRepository(product1);
        var orderRepository = new FakeOrderRepository();
        var missingProductId = Guid.NewGuid(); // Non-existent product ID

        var command = new CreateOrderCommand(
            customer.Id,
            [
                new CreateOrderItemInput(product1.Id, 2),
                new CreateOrderItemInput(missingProductId, 1) // Non-existent product ID
            ]);

        var handler = new CreateOrderHandler(
            new CreateOrderCommandValidator(),
            customerRepository,
            productRepository,
            orderRepository);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ProductsNotFoundException>(
            () => handler.HandleAsync(command, CancellationToken.None));

        Assert.Contains(missingProductId, exception.ProductIds);
        Assert.Null(orderRepository.SavedOrder);
    }
}