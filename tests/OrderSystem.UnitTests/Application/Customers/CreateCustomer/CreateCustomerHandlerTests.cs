namespace OrderSystem.UnitTests.Application.Customers.CreateCustomer;

using FluentValidation;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Customers.CreateCustomer;
using OrderSystem.Domain.Entities;

public sealed class CreateCustomerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAddCustomerAndSaveChanges_WhenCommandIsValid()
    {
        var repository = new FakeCustomerRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateCustomerHandler(
            new CreateCustomerCommandValidator(),
            repository,
            unitOfWork);
        var command = new CreateCustomerCommand("Test customer", "customer@example.com");

        var result = await handler.HandleAsync(command);

        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Email, result.Email);
        Assert.Same(result, repository.AddedCustomer);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotPersist_WhenCommandIsInvalid()
    {
        var repository = new FakeCustomerRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateCustomerHandler(
            new CreateCustomerCommandValidator(),
            repository,
            unitOfWork);
        var command = new CreateCustomerCommand("", "not-an-email");

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(command));

        Assert.Null(repository.AddedCustomer);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public Customer? AddedCustomer { get; private set; }

        public Task<Customer?> GetByIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Customer?>(null);
        }

        public Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken = default)
        {
            AddedCustomer = customer;
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
