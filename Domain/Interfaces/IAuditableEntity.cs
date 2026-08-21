using Domain.Entities;

namespace Domain.Interfaces;

public interface IAuditableEntity
{
    Guid Id { get; set; }
    User AddedBy { get; set; }
    Guid AddedById { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
