using Application.Configs;
using Application.Interfaces;
using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
namespace Infrastructure.Services;

public class LogedInUserData(
    IHttpContextAccessor httpContextAccessor, IOptions<SystemUserSettings> systemUserOptions, IServiceProvider serviceProvider)
    : IScopedService, ILogedInUserData
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IOptions<SystemUserSettings> _systemUserOptions = systemUserOptions;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private Guid? _systemUserId;
    private Guid? _systemTenantId;

    private ClaimsPrincipal? AuthenticatedUser =>  _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated
    {
        get
        {
            return _systemUserId.HasValue ||
            AuthenticatedUser?.Identity?.IsAuthenticated == true;

        }
    }

    public Guid UserId
    {
        get
        {
            if (_systemUserId.HasValue)
                return _systemUserId.Value;

            var userIdClaim = AuthenticatedUser?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }
    }

    public Guid TenantId
    {
        get
        {
            if (_systemTenantId.HasValue)
                return _systemTenantId.Value;

            var tenantClaim = AuthenticatedUser?.FindFirst("tenantId")?.Value;

            if (Guid.TryParse(tenantClaim, out var tenantId))
                return tenantId;

            throw new InvalidOperationException(
                "The current user doesn't belong to any tenant.");
        }
    }

    public bool IsOwner()
    {
        if (_systemUserId.HasValue)
            return true;

        var claim = AuthenticatedUser?.FindFirst("isOwner")?.Value;
        return bool.TryParse(claim, out var isOwner) && isOwner;
    }

    public string? UserEmail => _systemUserId.HasValue ? _systemUserOptions.Value.Email :
        AuthenticatedUser?.FindFirstValue(ClaimTypes.Email);

    public string? UserName => _systemUserId.HasValue ? _systemUserOptions.Value.Username :
        AuthenticatedUser?.FindFirstValue(ClaimTypes.Name);

    public string? UserRole => _systemUserId.HasValue ? Role.COFOUNDERS.OWNER.ToString() :
        AuthenticatedUser?.FindFirstValue(ClaimTypes.Role);

    public void UseSystemUser(Guid userId = default, Guid tenantId = default)
    {
        _systemUserId = userId;
        _systemTenantId = tenantId;
    }

    public async Task UseSystemUser(Guid tenantId)
    {
        var userRepo = _serviceProvider.GetRequiredService<IUserAuthQueries>();
        var systemUser = await userRepo.FindThisAsync(p => p.Email == _systemUserOptions.Value.Email);
        UseSystemUser(systemUser!.Id, tenantId);
    }
    public async Task UseSystemUser()
    {
        var userRepo = _serviceProvider.GetRequiredService<IUserAuthQueries>();
        var systemUser = await userRepo.FindThisAsync(p => p.Email == _systemUserOptions.Value.Email);
        UseSystemUser(systemUser!.Id, systemUser.TenantId);
    }
}