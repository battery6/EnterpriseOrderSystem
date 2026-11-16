namespace OrderSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Application.Orders.CreateOrder;
using OrderSystem.Application.Orders.GetOrder;
using OrderSystem.Domain.Entities;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(
    CreateOrderHandler createOrder,
    GetOrderHandler getOrder
) : ControllerBase
{
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
        order.GetTotalPrice(),
        items);
    }
}