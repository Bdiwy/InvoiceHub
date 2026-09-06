using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Entites;

public class BaseDomainEntity : ITenantEntity , IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }

    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Guid? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual User? UpdatedBy { get; set; }
    public virtual User CreatedBy { get; set; } = null!;
    public virtual User? DeletedBy { get; set; }
}
