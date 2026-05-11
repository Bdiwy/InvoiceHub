using Domain.Interfaces;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public class LogedInUserData(IHttpContextAccessor _httpContextAccessor) : IScopedService , ILogedInUserData
{
    private ClaimsPrincipal? AuthenticatedUser => _httpContextAccessor.HttpContext?.User;
    public bool IsAuthenticated => AuthenticatedUser?.Identity?.IsAuthenticated ?? false;

    public Guid UserId => IsAuthenticated
        ? Guid.Parse(AuthenticatedUser!.FindFirst(ClaimTypes.NameIdentifier)!.Value)
        : throw new UnauthorizedAccessException("User is not authenticated");

    public Guid TenantId => IsAuthenticated
        ? Guid.Parse(AuthenticatedUser!.FindFirst("tenantId")!.Value)
        : throw new InvalidOperationException("This user Didn't belongs to any company, tenantId never should be an empty !!!");

    public bool IsOwner()
    {
        var claim = AuthenticatedUser?.FindFirst("isOwner")?.Value;
        return bool.TryParse(claim, out var isOwner) && isOwner;
    }

    public string? UserEmail => AuthenticatedUser?.FindFirstValue(ClaimTypes.Email);
    public string? UserName => AuthenticatedUser?.FindFirstValue(ClaimTypes.Name);
    public string? UserRole => AuthenticatedUser?.FindFirstValue(ClaimTypes.Role);
}