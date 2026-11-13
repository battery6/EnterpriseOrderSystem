namespace OrderSystem.Application.Abstractions.Persistence;
using OrderSystem.Domain.Entities;

public interface IOrderRepository
{
    Task AddAsync(
        Order order, 
        CancellationToken cancellationToken = default);
}