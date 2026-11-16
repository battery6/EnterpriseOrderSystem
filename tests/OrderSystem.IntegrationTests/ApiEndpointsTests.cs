namespace OrderSystem.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;

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
        Assert.Equal(created.Items, retrieved.Items);
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
