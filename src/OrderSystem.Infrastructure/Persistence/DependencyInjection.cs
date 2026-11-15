namespace OrderSystem.Infrastructure.Persistance;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Infrastructure.Persistence;
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
}