namespace OrderSystem.Application.Orders.AddOrderItem;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;
using OrderSystem.Domain.Exceptions;

public sealed class AddOrderItemHandler(
    IValidator<AddOrderItemCommand> validator,
    IProductRepository products,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<AddOrderItemCommand> _validator = validator;
    private readonly IProductRepository _products = products;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Order> HandleAsync(
        AddOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orders.GetByIdAsync(
            command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        var product = await _products.GetByIdAsync(
            command.ProductId, cancellationToken) 
            ?? throw new ProductNotFoundException(command.ProductId);

        if(product.IsArchived)
        {
            throw new ProductArchivedException(command.ProductId);
        }

        order.AddItem(product, command.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}
