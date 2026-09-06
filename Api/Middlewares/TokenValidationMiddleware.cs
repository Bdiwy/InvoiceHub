using Api.Middlewares;
using Application.Exceptions;
using Domain.Entities;
using InvoiceHub.Application.Requests.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace InvoiceHub.Api.Middlewares;

public class TokenValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (!ShouldValidateToken(context))
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
            throw new UnauthorizedException();

        var token = ExtractBearerToken(context);
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException("Missing bearer token.");

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid token subject.");

        var deviceType = ResolveDeviceType(context.Request.Headers["X-Device-Type"].ToString());
        if (deviceType is null)
            throw new UnauthorizedException("Unsupported device type.");

        var storedToken = await dbContext.Set<AccessAndRefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.Token == token &&
                t.UserId == userId &&
                t.DeviceType == deviceType.Value);

        if (
            storedToken is null || 
            storedToken.IsRevoked || 
            storedToken.TokenExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedException();
        
        await next(context);
    }

    private static bool ShouldValidateToken(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var routeEndpoint = endpoint as RouteEndpoint;

        if (routeEndpoint is null || endpoint is null)
            return false;

        var route = routeEndpoint.RoutePattern.RawText;
        if (string.Equals(route, "api/auth/login", StringComparison.OrdinalIgnoreCase)||
            string.Equals(route, "api/auth/register", StringComparison.OrdinalIgnoreCase))
            return false;

        var allowsAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;
        if (allowsAnonymous)
            return false;

        return true;
    }

    private static string? ExtractBearerToken(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        const string bearerPrefix = "Bearer ";
        if (!authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        return authHeader[bearerPrefix.Length..].Trim();
    }

    private static DeviceType? ResolveDeviceType(string headerValue)
    {
        var value = string.IsNullOrWhiteSpace(headerValue)
            ? nameof(DeviceType.WEB)
            : headerValue;

        return Enum.TryParse<DeviceType>(value, true, out var parsed) ? parsed : null;
    }
}

public static class TokenValidationHandlerMiddleware
{
    public static IApplicationBuilder TokenValidationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenValidationMiddleware>();
    }
}
