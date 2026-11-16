namespace OrderSystem.UnitTests.Application.Products.CreateProduct;

using FluentValidation;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Products.CreateProduct;
using OrderSystem.Domain.Entities;

public sealed class CreateProductHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAddProductAndSaveChanges_WhenCommandIsValid()
    {
        var repository = new FakeProductRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateProductHandler(
            new CreateProductCommandValidator(),
            repository,
            unitOfWork);
        var command = new CreateProductCommand("Test product", 12.50m);

        var result = await handler.HandleAsync(command);

        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.CurrentPrice, result.CurrentPrice);
        Assert.Same(result, repository.AddedProduct);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenCommandIsInvalid()
    {
        var repository = new FakeProductRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateProductHandler(
            new CreateProductCommandValidator(),
            repository,
            unitOfWork);
        var command = new CreateProductCommand("", -1m);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(command));

        Assert.Null(repository.AddedProduct);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public Product? AddedProduct { get; private set; }

        public Task<Product?> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Product?>(null);
        }

        public Task<IReadOnlyList<Product>> GetByIdsAsync(
            Guid[] productIds,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Product>>([]);
        }

        public Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            AddedProduct = product;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
