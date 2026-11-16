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
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<GetProductHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderHandler>();

        services.AddValidatorsFromAssembly(
            typeof(CreateOrderCommandValidator).Assembly);

        return services;
    }
}