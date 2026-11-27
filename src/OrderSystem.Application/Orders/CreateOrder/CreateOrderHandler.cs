namespace OrderSystem.Application.Orders.CreateOrder;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;
using OrderSystem.Domain.Exceptions;

public sealed class CreateOrderHandler(
    IValidator<CreateOrderCommand> validator,
    ICustomerRepository customers,
    IProductRepository products,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<CreateOrderCommand> _validator = validator;
    private readonly ICustomerRepository _customers = customers;
    private readonly IProductRepository _products = products;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Order> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customers.GetByIdAsync(
            command.CustomerId, cancellationToken) 
            ?? throw new CustomerNotFoundException(command.CustomerId);

        if(customer.IsArchived)
        {
            throw new CustomerArchivedException(command.CustomerId);
        }
        
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

        var archivedProduct = products.FirstOrDefault(
            product => product.IsArchived);
        
        if (archivedProduct is not null)
        {
            throw new ProductArchivedException(archivedProduct.Id);
        }

        var orderItems = command.OrderItems
            .GroupBy(item => item.ProductId)
            .Select(group => new OrderItem(
                productsById[group.Key],
                group.Sum(item => item.Quantity)))
        .ToList();

        var order = new Order(orderItems, customer);

        await _orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}
