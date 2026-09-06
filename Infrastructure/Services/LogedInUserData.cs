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

    private ClaimsPrincipal? AuthenticatedUser =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        _systemUserId.HasValue ||
        AuthenticatedUser?.Identity?.IsAuthenticated == true;

    public Guid UserId =>
        _systemUserId ??
        (IsAuthenticated
            ? Guid.Parse(
                AuthenticatedUser!
                    .FindFirst(ClaimTypes.NameIdentifier)!.Value)
            : throw new UnauthorizedAccessException(
                "User is not authenticated"));

    public Guid TenantId =>
        _systemTenantId ??
        (IsAuthenticated
            ? Guid.Parse(
                AuthenticatedUser!
                    .FindFirst("tenantId")!.Value)
            : throw new InvalidOperationException(
                "This user didn't belong to any tenant."));

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
        var _commonQueries = _serviceProvider.GetRequiredService<ICommonQueries<User>>();
        var systemUser = await _commonQueries.FetchFirstAsync(p => p.Email == _systemUserOptions.Value.Email);
        UseSystemUser(systemUser!.Id, tenantId);
    }
    public async Task UseSystemUser()
    {
        var _commonQueries = _serviceProvider.GetRequiredService<ICommonQueries<User>>();
        var systemUser = await _commonQueries.FetchFirstAsync(p => p.Email == _systemUserOptions.Value.Email);
        UseSystemUser(systemUser!.Id, systemUser.TenantId);
    }
}