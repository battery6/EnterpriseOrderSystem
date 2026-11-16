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
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<CreateOrderHandler>();

        services.AddValidatorsFromAssembly(
            typeof(CreateOrderCommandValidator).Assembly);

        return services;
    }
}