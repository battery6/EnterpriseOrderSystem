namespace OrderSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Application.Products.CreateProduct;
using OrderSystem.Application.Products.GetProduct;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    CreateProductHandler createProduct,
    GetProductHandler getProduct
) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await getProduct.HandleAsync(
            new GetProductQuery(id),
            cancellationToken);
        
        if(product is null)
        {
            return NotFound();
        }

        return Ok(new ProductResponse(product.Id, product.Name, product.CurrentPrice));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {

        var product = await createProduct.HandleAsync(
            new CreateProductCommand(request.Name, request.CurrentPrice),
            cancellationToken);

        var response = new ProductResponse(product.Id, product.Name, product.CurrentPrice);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            response);
    }
}