namespace OrderSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Application.Orders.CreateOrder;
using OrderSystem.Application.Orders.GetOrder;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Orders.AddOrderItem;
using OrderSystem.Application.Orders.RemoveOrderItem;
using OrderSystem.Application.Orders.CancelOrder;
using OrderSystem.Application.Orders.ChangeOrderItemQuantity;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(
    CreateOrderHandler createOrder,
    GetOrderHandler getOrder,
    AddOrderItemHandler addOrderItem,
    RemoveOrderItemHandler removeOrderItem,
    ChangeOrderItemQuantityHandler changeOrderItemQuantity,
    CancelOrderHandler cancelOrder
) : ControllerBase
{
    private static OrderResponse ToResponse(Order order)
    {
    var items = order.GetItems()
        .Select(item => new OrderItemResponse(
            item.Product.Id,
            item.Quantity,
            item.UnitPrice,
            item.GetTotalPrice()))
        .ToArray();

    return new OrderResponse(
        order.Id,
        order.Customer.Id,
        order.CreatedAt,
        order.Status,
        order.GetTotalPrice(),
        items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await getOrder.HandleAsync(
            new GetOrderQuery(id),
            cancellationToken);
        
        if(order is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {

        var command = new CreateOrderCommand(
            request.CustomerId,
            request.OrderItems
                .Select(item => new CreateOrderItemInput(
                    item.ProductId,
                    item.Quantity))
                .ToArray());

        var order = await createOrder.HandleAsync(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            ToResponse(order));
    }

    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<OrderResponse>> AddItem(
        Guid id,
        AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var order = await addOrderItem.HandleAsync(
            new AddOrderItemCommand(id, request.ProductId, request.Quantity),
            cancellationToken);
        
        return Ok(ToResponse(order));
    }

    [HttpPatch("{id:guid}/items/{productId:guid}")]
    public async Task<ActionResult<OrderResponse>> ChangeOrderItemQuantity(
        Guid id,
        Guid productId,
        ChangeOrderItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var order = await changeOrderItemQuantity.HandleAsync(
            new ChangeOrderItemQuantityCommand(id, productId, request.Quantity),
            cancellationToken);
        
        return Ok(ToResponse(order));
    }

    [HttpDelete("{id:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid id,
        Guid productId,
        CancellationToken cancellationToken)
    {
        await removeOrderItem.HandleAsync(
            new RemoveOrderItemCommand(id, productId),
            cancellationToken);
        
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await cancelOrder.HandleAsync(
            new CancelOrderCommand(id),
            cancellationToken);
        
        return Ok(ToResponse(order));
    }
}