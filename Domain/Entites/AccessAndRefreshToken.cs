using Domain.Interfaces;

namespace Domain.Entities;

public class AccessAndRefreshToken : ITenantEntity
{
    private AccessAndRefreshToken(){}
    public AccessAndRefreshToken(Guid userId,Guid tenantId, DeviceType deviceType, string token, string refreshToken, DateTime tokenExpiry, DateTime refreshTokenExpiry)
    {
       TenantId = tenantId;
       DeviceType = deviceType;
       Token = token;
       RefreshToken = refreshToken;
       UserId = userId;
       TokenExpiresAt = tokenExpiry;
       RefreshTokenExpiresAt = refreshTokenExpiry;
    }
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DeviceType DeviceType { get; set; } = DeviceType.WEB;
    
    public bool IsRevoked { get; set; } = false;
    public DateTime TokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= RefreshTokenExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

public enum DeviceType
{
    WEB,
    MOBILE
}
public static class DeviceTypeExtensions
{
    /// <summary>
    /// Checks if the provided string matches a valid DeviceType.
    /// </summary>
    public static bool TryThisTypeAndMatch(this string deviceType)
    {
        // Enum.TryParse handles the conversion and returns true if it exists.
        // ignoreCase: true allows "web" to match DeviceType.WEB.
        return Enum.TryParse(deviceType, true, out DeviceType _);
    }
}