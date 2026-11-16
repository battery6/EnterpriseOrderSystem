namespace OrderSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Application.Customers.CreateCustomer;
using OrderSystem.Application.Customers.GetCustomer;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(
    CreateCustomerHandler createCustomer,
    GetCustomerHandler getCustomer
) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await getCustomer.HandleAsync(
            new GetCustomerQuery(id), 
            cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(new CustomerResponse(customer.Id, customer.Name, customer.Email));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {

        var customer = await createCustomer.HandleAsync(
            new CreateCustomerCommand(request.Name, request.Email),
            cancellationToken);

        var response = new CustomerResponse(customer.Id, customer.Name, customer.Email);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            response);
    }
}