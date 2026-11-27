namespace OrderSystem.Infrastructure.Persistence.Repositories;

using OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class ProductRepository(OrderSystemDbContext dbContext) : IProductRepository
{
    private readonly OrderSystemDbContext _dbContext = dbContext;

    public async Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
       return await _dbContext.Products.FindAsync([productId], cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(
        Guid[] productIds, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }
}