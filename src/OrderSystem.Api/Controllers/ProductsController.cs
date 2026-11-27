namespace OrderSystem.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Application.Products.CreateProduct;
using OrderSystem.Application.Products.GetProduct;
using OrderSystem.Application.Products.ChangeProductPrice;
using OrderSystem.Application.Products.DeleteProduct;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    CreateProductHandler createProduct,
    GetProductHandler getProduct,
    ChangeProductPriceHandler changeProductPrice,
    DeleteProductHandler deleteProduct
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

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> ChangePrice(
        Guid id,
        ChangeProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        var Product = await changeProductPrice.HandleAsync(
            new ChangeProductPriceCommand(id, request.CurrentPrice),
            cancellationToken);
        
        return Ok(new ProductResponse(
            Product.Id, 
            Product.Name, 
            Product.CurrentPrice));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await deleteProduct.HandleAsync(
            new DeleteProductCommand(id),
            cancellationToken);

        return NoContent();
    }
}