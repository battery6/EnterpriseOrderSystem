namespace OrderSystem.Infrastructure.Persistence;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Application.Orders.CreateOrder;
using OrderSystem.Application.Products.CreateProduct;
using OrderSystem.Application.Customers.CreateCustomer;
using OrderSystem.Infrastructure.Persistence.Repositories;
using OrderSystem.Application.Customers.GetCustomer;
using OrderSystem.Application.Products.GetProduct;
using OrderSystem.Application.Orders.GetOrder;
using OrderSystem.Application.Products.ChangeProductPrice;
using OrderSystem.Application.Customers.ChangeCustomerEmail;
using OrderSystem.Application.Products.DeleteProduct;
using OrderSystem.Application.Customers.DeleteCustomer;
using OrderSystem.Application.Orders.AddOrderItem;
using OrderSystem.Application.Orders.RemoveOrderItem;
using OrderSystem.Application.Orders.ChangeOrderItemQuantity;
using OrderSystem.Application.Orders.CancelOrder;

public static class DependencyInjection 
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("OrderSystem")
            ?? throw new InvalidOperationException(
                "Connection string 'OrderSystem' was not configured.");

        services.AddDbContext<OrderSystemDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork>(
            provider => provider.GetRequiredService<OrderSystemDbContext>());

        return services;
    }

    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateCustomerHandler>();
        services.AddScoped<GetCustomerHandler>();
        services.AddScoped<ChangeCustomerEmailHandler>();
        services.AddScoped<DeleteCustomerHandler>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<GetProductHandler>();
        services.AddScoped<ChangeProductPriceHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<AddOrderItemHandler>();
        services.AddScoped<RemoveOrderItemHandler>();
        services.AddScoped<ChangeOrderItemQuantityHandler>();
        services.AddScoped<CancelOrderHandler>();

        services.AddValidatorsFromAssembly(
            typeof(CreateOrderCommandValidator).Assembly);

        return services;
    }
}