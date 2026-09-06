namespace Application.Interfaces;

public interface ILogedInUserData
{
    Guid UserId { get; }
    Guid TenantId { get; }
    bool IsOwner();
    bool IsAuthenticated { get; }
    string? UserEmail { get; }
    string? UserName { get; }
    string? UserRole { get; }
    void UseSystemUser(Guid userId = default, Guid tenantId = default);
}