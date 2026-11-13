namespace OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetByIdsAsync(
        Guid[] productIds, 
        CancellationToken cancellationToken = default);
}