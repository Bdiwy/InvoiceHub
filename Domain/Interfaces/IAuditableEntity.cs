namespace Domain.Interfaces;

public interface IAuditableEntity
{
    Guid AddedById { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
