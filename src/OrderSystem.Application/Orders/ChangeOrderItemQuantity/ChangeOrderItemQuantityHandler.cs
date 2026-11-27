namespace OrderSystem.Application.Orders.ChangeOrderItemQuantity;

using FluentValidation;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Common.Exceptions;

public sealed class ChangeOrderItemQuantityHandler(
    IValidator<ChangeOrderItemQuantityCommand> validator,
    IOrderRepository orders,
    IUnitOfWork unitOfWork)
{
    private readonly IValidator<ChangeOrderItemQuantityCommand> _validator = validator;
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Order> HandleAsync(
        ChangeOrderItemQuantityCommand command,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orders.GetByIdAsync(
            command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.ChangeItemQuantity(command.ProductId, command.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}
