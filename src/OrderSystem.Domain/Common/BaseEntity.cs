namespace OrderSystem.Domain.Common;

// BaseEntity class serves as a base class for all entities in the domain, providing a unique identifier.
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public abstract class TenantEntity : BaseEntity
{
    public string TenantId { get; protected set; } = string.Empty;

}

