namespace OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
        
    Task<IReadOnlyList<Product>> GetByIdsAsync(
        Guid[] productIds, 
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);
}