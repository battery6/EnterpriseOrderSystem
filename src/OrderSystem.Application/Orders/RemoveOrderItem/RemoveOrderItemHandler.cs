namespace OrderSystem.Application.Orders.RemoveOrderItem;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class RemoveOrderItemHandler(
    IValidator<RemoveOrderItemCommand> validator,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<RemoveOrderItemCommand> _validator = validator;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Order> HandleAsync(
        RemoveOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orders.GetByIdAsync(
            command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.RemoveItem(command.ProductId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}
