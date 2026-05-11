using Domain.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
public class TenantService(IHttpContextAccessor httpContextAccessor) : ISingletonService, ITenantService
    {
        public Guid? GetTenantId()
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")?.Value;
            return Guid.TryParse(claim, out var tenantId) ? tenantId : null;
        }
    }
}