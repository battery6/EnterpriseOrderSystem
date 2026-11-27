namespace OrderSystem.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Domain.Enums;

public sealed class ApiEndpointsTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;

    public ApiEndpointsTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedResourceAndGetReturnsIt()
    {
        await _fixture.ResetDatabaseAsync();
        var request = new CreateCustomerRequest(
            "Test customer",
            $"customer-{Guid.NewGuid():N}@example.com");

        using var createResponse = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<CustomerResponse>(createResponse);
        var location = createResponse.Headers.Location
            ?? throw new InvalidOperationException("The response did not include a Location header.");

        Assert.Equal($"/api/customers/{created.Id}", location.AbsolutePath);

        using var getResponse = await _fixture.Client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await ReadJsonAsync<CustomerResponse>(getResponse);
        Assert.Equal(created, retrieved);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.GetAsync(
            $"/api/customers/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("", "not-an-email"));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsBadRequest_WhenApplicationValidationFails()
    {
        await _fixture.ResetDatabaseAsync();
        var request = new CreateCustomerRequest(
            new string('n', 101),
            "customer@example.com");

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            request);

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsConflict_WhenEmailAlreadyExists()
    {
        await _fixture.ResetDatabaseAsync();
        var request = new CreateCustomerRequest(
            "Test customer",
            "duplicate@example.com");

        using var firstResponse = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            request);
        using var secondResponse = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        await AssertProblemDetailsAsync(
            secondResponse,
            HttpStatusCode.Conflict,
            "Email already in use");
    }

    [Fact]
    public async Task ChangeCustomerEmail_ReturnsUpdatedCustomer()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var newEmail = $"updated-{Guid.NewGuid():N}@example.com";

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}",
            new ChangeCustomerEmailRequest(newEmail));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadJsonAsync<CustomerResponse>(response);
        Assert.Equal(customer.Id, updated.Id);
        Assert.Equal(newEmail, updated.Email);
    }

    [Fact]
    public async Task ChangeCustomerEmail_ReturnsBadRequest_WhenEmailIsInvalid()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}",
            new ChangeCustomerEmailRequest("not-an-email"));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangeCustomerEmail_ReturnsConflict_WhenEmailIsAlreadyUsed()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var otherCustomer = await CreateCustomerAsync();

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}",
            new ChangeCustomerEmailRequest(otherCustomer.Email));

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.Conflict,
            "Email already in use");
    }

    [Fact]
    public async Task DeleteCustomer_HardDeletesCustomerWithoutOrders()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();

        using var deleteResponse = await _fixture.Client.DeleteAsync(
            $"/api/customers/{customer.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using var getResponse = await _fixture.Client.GetAsync(
            $"/api/customers/{customer.Id}");
        await AssertProblemDetailsAsync(getResponse, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_ArchivesCustomerWithOrdersAndPreservesOrder()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 2));

        using var deleteResponse = await _fixture.Client.DeleteAsync(
            $"/api/customers/{customer.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using var customerResponse = await _fixture.Client.GetAsync(
            $"/api/customers/{customer.Id}");
        await AssertProblemDetailsAsync(customerResponse, HttpStatusCode.NotFound);

        using var orderResponse = await _fixture.Client.GetAsync(
            $"/api/orders/{order.Id}");
        Assert.Equal(HttpStatusCode.OK, orderResponse.StatusCode);
        var persistedOrder = await ReadJsonAsync<OrderResponse>(orderResponse);
        Assert.Equal(customer.Id, persistedOrder.CustomerId);
        Assert.Single(persistedOrder.Items);

        using var changeResponse = await _fixture.Client.PatchAsJsonAsync(
            $"/api/customers/{customer.Id}",
            new ChangeCustomerEmailRequest($"new-{Guid.NewGuid():N}@example.com"));
        await AssertProblemDetailsAsync(
            changeResponse,
            HttpStatusCode.Conflict,
            "Customer is archived");
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedResourceAndGetReturnsIt()
    {
        await _fixture.ResetDatabaseAsync();
        var request = new CreateProductRequest("Test product", 12.50m);

        using var createResponse = await _fixture.Client.PostAsJsonAsync(
            "/api/products",
            request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<ProductResponse>(createResponse);
        var location = createResponse.Headers.Location
            ?? throw new InvalidOperationException("The response did not include a Location header.");

        Assert.Equal($"/api/products/{created.Id}", location.AbsolutePath);

        using var getResponse = await _fixture.Client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await ReadJsonAsync<ProductResponse>(getResponse);
        Assert.Equal(created, retrieved);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.GetAsync(
            $"/api/products/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenPriceIsNegative()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Test product", -1m));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangeProductPrice_ReturnsUpdatedProduct()
    {
        await _fixture.ResetDatabaseAsync();
        var product = await CreateProductAsync(12.50m);

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/products/{product.Id}",
            new ChangeProductPriceRequest(8.25m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadJsonAsync<ProductResponse>(response);
        Assert.Equal(product.Id, updated.Id);
        Assert.Equal(8.25m, updated.CurrentPrice);
    }

    [Fact]
    public async Task ChangeProductPrice_ReturnsBadRequest_WhenPriceIsNegative()
    {
        await _fixture.ResetDatabaseAsync();
        var product = await CreateProductAsync(12.50m);

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/products/{product.Id}",
            new ChangeProductPriceRequest(-1m));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_HardDeletesProductWithoutOrderItems()
    {
        await _fixture.ResetDatabaseAsync();
        var product = await CreateProductAsync(12.50m);

        using var deleteResponse = await _fixture.Client.DeleteAsync(
            $"/api/products/{product.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        using var getResponse = await _fixture.Client.GetAsync(
            $"/api/products/{product.Id}");
        await AssertProblemDetailsAsync(getResponse, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedResourceAndGetReturnsItems()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(12.50m);
        var request = new CreateOrderRequest(
            customer.Id,
            [new CreateOrderItemRequest(product.Id, 2)]);

        using var createResponse = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadJsonAsync<OrderResponse>(createResponse);
        var location = createResponse.Headers.Location
            ?? throw new InvalidOperationException("The response did not include a Location header.");

        Assert.Equal($"/api/orders/{created.Id}", location.AbsolutePath);
        Assert.Equal(customer.Id, created.CustomerId);
        Assert.Equal(OrderStatus.Placed, created.Status);
        Assert.Equal(25m, created.TotalPrice);
        var createdItem = Assert.Single(created.Items);
        Assert.Equal(product.Id, createdItem.ProductId);
        Assert.Equal(2, createdItem.Quantity);
        Assert.Equal(12.50m, createdItem.UnitPrice);
        Assert.Equal(25m, createdItem.TotalPrice);

        using var getResponse = await _fixture.Client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await ReadJsonAsync<OrderResponse>(getResponse);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.CustomerId, retrieved.CustomerId);
        Assert.Equal(created.TotalPrice, retrieved.TotalPrice);
        Assert.Equal(created.Status, retrieved.Status);
        Assert.Equal(created.Items, retrieved.Items);
    }

    [Fact]
    public async Task CreateOrder_CombinesDuplicateProductLines()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(3m);

        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 2),
            new CreateOrderItemRequest(product.Id, 3));

        var item = Assert.Single(order.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(5, item.Quantity);
        Assert.Equal(15m, order.TotalPrice);
    }

    [Fact]
    public async Task AddOrderItem_IncrementsExistingLineAndAddsNewProduct()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var existingProduct = await CreateProductAsync(5m);
        var newProduct = await CreateProductAsync(2m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(existingProduct.Id, 2));

        using var priceResponse = await _fixture.Client.PatchAsJsonAsync(
            $"/api/products/{existingProduct.Id}",
            new ChangeProductPriceRequest(9m));
        Assert.Equal(HttpStatusCode.OK, priceResponse.StatusCode);

        using var addExistingResponse = await _fixture.Client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/items",
            new AddOrderItemRequest(existingProduct.Id, 3));

        Assert.Equal(HttpStatusCode.OK, addExistingResponse.StatusCode);
        var afterExistingAdd = await ReadJsonAsync<OrderResponse>(addExistingResponse);
        var existingLine = Assert.Single(afterExistingAdd.Items);
        Assert.Equal(5, existingLine.Quantity);
        Assert.Equal(5m, existingLine.UnitPrice);

        using var addNewResponse = await _fixture.Client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/items",
            new AddOrderItemRequest(newProduct.Id, 1));

        Assert.Equal(HttpStatusCode.OK, addNewResponse.StatusCode);
        var updated = await ReadJsonAsync<OrderResponse>(addNewResponse);
        Assert.Equal(2, updated.Items.Count);
        Assert.Equal(27m, updated.TotalPrice);
        Assert.Contains(
            updated.Items,
            item => item.ProductId == newProduct.Id
                && item.Quantity == 1
                && item.UnitPrice == 2m);
    }

    [Fact]
    public async Task AddOrderItem_ReturnsBadRequest_WhenQuantityIsNotPositive()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 1));

        using var response = await _fixture.Client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/items",
            new AddOrderItemRequest(product.Id, 0));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_ArchivesProductReferencedByOrder()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(7.50m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 2));

        using var deleteResponse = await _fixture.Client.DeleteAsync(
            $"/api/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var productResponse = await _fixture.Client.GetAsync(
            $"/api/products/{product.Id}");
        await AssertProblemDetailsAsync(productResponse, HttpStatusCode.NotFound);

        using var orderResponse = await _fixture.Client.GetAsync(
            $"/api/orders/{order.Id}");
        Assert.Equal(HttpStatusCode.OK, orderResponse.StatusCode);
        var preservedOrder = await ReadJsonAsync<OrderResponse>(orderResponse);
        var item = Assert.Single(preservedOrder.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(7.50m, item.UnitPrice);
        Assert.Equal(15m, item.TotalPrice);

        using var addResponse = await _fixture.Client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/items",
            new AddOrderItemRequest(product.Id, 1));
        await AssertProblemDetailsAsync(
            addResponse,
            HttpStatusCode.Conflict,
            "Product is archived");
    }

    [Fact]
    public async Task ChangeOrderItemQuantity_UpdatesLineAndTotal()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 2));

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/orders/{order.Id}/items/{product.Id}",
            new ChangeOrderItemQuantityRequest(5));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadJsonAsync<OrderResponse>(response);
        var item = Assert.Single(updated.Items);
        Assert.Equal(5, item.Quantity);
        Assert.Equal(20m, updated.TotalPrice);
    }

    [Fact]
    public async Task ChangeOrderItemQuantity_ReturnsBadRequest_WhenQuantityIsNotPositive()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 1));

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/orders/{order.Id}/items/{product.Id}",
            new ChangeOrderItemQuantityRequest(0));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveOrderItem_RemovesLineWhenAnotherLineRemains()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var removedProduct = await CreateProductAsync(4m);
        var remainingProduct = await CreateProductAsync(6m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(removedProduct.Id, 1),
            new CreateOrderItemRequest(remainingProduct.Id, 2));

        using var response = await _fixture.Client.DeleteAsync(
            $"/api/orders/{order.Id}/items/{removedProduct.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var getResponse = await _fixture.Client.GetAsync(
            $"/api/orders/{order.Id}");
        var updated = await ReadJsonAsync<OrderResponse>(getResponse);
        var item = Assert.Single(updated.Items);
        Assert.Equal(remainingProduct.Id, item.ProductId);
        Assert.Equal(12m, updated.TotalPrice);
    }

    [Fact]
    public async Task RemoveOrderItem_ReturnsConflictWhenItWouldEmptyOrder()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 1));

        using var response = await _fixture.Client.DeleteAsync(
            $"/api/orders/{order.Id}/items/{product.Id}");

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.Conflict,
            "Order cannot be empty");
    }

    [Fact]
    public async Task CancelOrder_PersistsCancelledStatusAndRejectsFurtherEdits()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 1));

        using var cancelResponse = await _fixture.Client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            content: null);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        var cancelled = await ReadJsonAsync<OrderResponse>(cancelResponse);
        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);

        using var getResponse = await _fixture.Client.GetAsync(
            $"/api/orders/{order.Id}");
        var persisted = await ReadJsonAsync<OrderResponse>(getResponse);
        Assert.Equal(OrderStatus.Cancelled, persisted.Status);
        Assert.Single(persisted.Items);

        using var addResponse = await _fixture.Client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/items",
            new AddOrderItemRequest(product.Id, 1));
        await AssertProblemDetailsAsync(
            addResponse,
            HttpStatusCode.Conflict,
            "Order cannot be modified");

        using var secondCancelResponse = await _fixture.Client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            content: null);
        await AssertProblemDetailsAsync(
            secondCancelResponse,
            HttpStatusCode.Conflict,
            "Order status change not possible");
    }

    [Fact]
    public async Task ChangeOrderItemQuantity_ReturnsNotFound_WhenProductIsNotInOrder()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(4m);
        var order = await CreateOrderAsync(
            customer.Id,
            new CreateOrderItemRequest(product.Id, 1));

        using var response = await _fixture.Client.PatchAsJsonAsync(
            $"/api/orders/{order.Id}/items/{Guid.NewGuid()}",
            new ChangeOrderItemQuantityRequest(2));

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound,
            "Order item not found");
    }

    [Fact]
    public async Task CancelOrder_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.PostAsync(
            $"/api/orders/{Guid.NewGuid()}/cancel",
            content: null);

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.GetAsync(
            $"/api/orders/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenQuantityIsInvalid()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                customer.Id,
                [new CreateOrderItemRequest(Guid.NewGuid(), 0)]));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_ReturnsBadRequest_WhenOrderHasNoItems()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(Guid.NewGuid(), []));

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                Guid.NewGuid(),
                [new CreateOrderItemRequest(Guid.NewGuid(), 1)]));

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound,
            "Customer not found");
    }

    [Fact]
    public async Task CreateOrder_ReturnsNotFound_WhenProductDoesNotExist()
    {
        await _fixture.ResetDatabaseAsync();
        var customer = await CreateCustomerAsync();

        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                customer.Id,
                [new CreateOrderItemRequest(Guid.NewGuid(), 1)]));

        await AssertProblemDetailsAsync(
            response,
            HttpStatusCode.NotFound,
            "Products not found");
    }

    [Fact]
    public async Task OpenApiDocument_IsAvailable()
    {
        using var response = await _fixture.Client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<CustomerResponse> CreateCustomerAsync()
    {
        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest(
                "Test customer",
                $"customer-{Guid.NewGuid():N}@example.com"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadJsonAsync<CustomerResponse>(response);
    }

    private async Task<ProductResponse> CreateProductAsync(decimal price)
    {
        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Test product", price));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadJsonAsync<ProductResponse>(response);
    }

    private async Task<OrderResponse> CreateOrderAsync(
        Guid customerId,
        params CreateOrderItemRequest[] items)
    {
        using var response = await _fixture.Client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(customerId, items.ToList()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadJsonAsync<OrderResponse>(response);
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("The response did not contain JSON.");
    }

    private static async Task AssertProblemDetailsAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string? expectedTitle = null)
    {
        Assert.Equal(expectedStatus, response.StatusCode);

        var problemDetails = await ReadJsonAsync<ProblemDetails>(response);

        Assert.Equal((int)expectedStatus, problemDetails.Status);
        Assert.False(string.IsNullOrWhiteSpace(problemDetails.Title));
        if (expectedTitle is not null)
        {
            Assert.Equal(expectedTitle, problemDetails.Title);
        }
    }
}
