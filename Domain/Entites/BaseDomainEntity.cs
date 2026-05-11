using Domain.Entities;
using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entites;

public class BaseDomainEntity : ITenantEntity , IAuditableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public Guid AddedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual User AddedBy { get; set; } = null!;
}
