using Domain.Entities;

namespace Domain.Interfaces;

public interface IAuditableEntity
{
    Guid Id { get; set; }
    Guid CreatedById { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedById { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedById { get; set; }
    User CreatedBy { get; set; }
    User? UpdatedBy { get; set; }
    User? DeletedBy { get; set; }

}
