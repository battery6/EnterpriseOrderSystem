namespace OrderSystem.UnitTests.Infrastructure.Persistence;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application.Customers.CreateCustomer;
using OrderSystem.Application.Orders.CreateOrder;
using OrderSystem.Application.Products.CreateProduct;
using OrderSystem.Infrastructure.Persistence;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersHandlersAndValidators()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        AssertRegisteredScoped<CreateCustomerHandler>(services);
        AssertRegisteredScoped<CreateProductHandler>(services);
        AssertRegisteredScoped<CreateOrderHandler>(services);

        using var provider = services.BuildServiceProvider();
        Assert.IsType<CreateCustomerCommandValidator>(
            provider.GetRequiredService<IValidator<CreateCustomerCommand>>());
        Assert.IsType<CreateProductCommandValidator>(
            provider.GetRequiredService<IValidator<CreateProductCommand>>());
        Assert.IsType<CreateOrderCommandValidator>(
            provider.GetRequiredService<IValidator<CreateOrderCommand>>());
    }

    private static void AssertRegisteredScoped<TService>(IServiceCollection services)
    {
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(TService)
                && descriptor.Lifetime == ServiceLifetime.Scoped);
    }
}
