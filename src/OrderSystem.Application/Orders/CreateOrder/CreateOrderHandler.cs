namespace OrderSystem.Application.Orders.CreateOrder;
using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class CreateOrderHandler(
    IValidator<CreateOrderCommand> validator,
    ICustomerRepository customers,
    IProductRepository products,
    IOrderRepository orders)
{
    private readonly IValidator<CreateOrderCommand> _validator = validator;
    private readonly ICustomerRepository _customers = customers;
    private readonly IProductRepository _products = products;
    private readonly IOrderRepository _orders = orders;

    public async Task<Order> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customers.GetByIdAsync(
            command.CustomerId, cancellationToken) 
            ?? throw new CustomerNotFoundException(command.CustomerId);
        
        var productIds = command.OrderItems
            .Select(item => item.ProductId)
            .Distinct()
            .ToArray();

        var products = await _products.GetByIdsAsync(
            productIds, cancellationToken);

        var productsById = products.ToDictionary(product => product.Id);
        
        var missingIds = productIds
            .Where(id => !productsById.ContainsKey(id))
            .ToArray();

        if (missingIds.Length > 0)
            throw new ProductsNotFoundException(missingIds);

        var order = new Order([], customer);

        foreach (var item in command.OrderItems)
            order.AddItem(productsById[item.ProductId], item.Quantity);

        await _orders.AddAsync(order, cancellationToken);

        return order;
    }
}
