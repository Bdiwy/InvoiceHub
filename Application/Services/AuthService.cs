using Application.Interfaces;
using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Interfaces;
using InvoiceHub.Application.Requests;
using InvoiceHub.Application.Requests.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;
public class AuthService(
    IJwtTokenGenerator jwtTokenGenerator,
    IUserAuthQueries userAuthQueries,
    ICommonQueries<User> userRepo,
    ICommonQueries<AccessAndRefreshToken> tokenQueries,
    ICommonCommands<AccessAndRefreshToken> tokenRepo,
    ICommonCommands<User> userCommandsRepo,
    ICommonCommands<Role> roleCommandsRepo,
    IConfiguration _config) : IScopedService , IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? apiKey, string deviceType, CancellationToken ct)
    {
        if (!TryParseDeviceType(deviceType, out var parsedDeviceType))
            return AuthResponseDto.Failure("Unsupported device type.");

        var user = await GetAndValidateUserCredentialsAsync(request, ct);
        if (user is null)
            return AuthResponseDto.Failure("Invalid email or password.");

        if (!ValidateMobileApiKey(parsedDeviceType, apiKey))
            return AuthResponseDto.Failure("Invalid mobile security key.");

        await RevokeOldDeviceTokenAsync(user.Id, parsedDeviceType, ct);

        var token = jwtTokenGenerator.GenerateToken(user);
        var refreshToken = Guid.NewGuid().ToString("N");
        var (tokenExpiry, refreshTokenExpiry) = GetExpiryWindows(parsedDeviceType);

        var tokenRegistration = CreateTokenEntity(user.Id, user.TenantId,parsedDeviceType, token, refreshToken, tokenExpiry, refreshTokenExpiry);
        await tokenRepo.SaveMeAsync(tokenRegistration, ct);

        return AuthResponseDto.SuccessLogin(token, refreshToken, user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        User? existingUser = await userRepo.FetchFirstAsync(u => u.Email == request.Email, ct);
        
        if (existingUser is not null)
            return new AuthResponseDto(IsSuccess: false, Message: "Email already in use.");
        
        var newTenantId = Guid.NewGuid();
        var ownerRole = CreateNewOwnerRole(newTenantId);
        await roleCommandsRepo.SaveMeAsync(ownerRole, ct);

        var newUser = CreateUserEntity(request.Username, request.Email, true, newTenantId, request.PhoneNumber, ownerRole.Id , ownerRole );
        newUser.Password = new PasswordHasher<User>().HashPassword(newUser, request.Password);

        await userCommandsRepo.SaveMeAsync(newUser, ct);
        return AuthResponseDto.SuccessRegister();
    }

    public async Task<AuthResponseDto> LogoutAsync(Guid userId, string? apiKey, string deviceType, CancellationToken ct)
    {
        if (!TryParseDeviceType(deviceType, out var parsedDeviceType))
            return AuthResponseDto.Failure("Unsupported device type.");
        
        if (!ValidateMobileApiKey(parsedDeviceType, apiKey))
            return AuthResponseDto.Failure("Invalid mobile security key.");

        await RevokeOldDeviceTokenAsync(userId, parsedDeviceType, ct);
        return AuthResponseDto.SuccessLogout();
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? apiKey, string deviceType, CancellationToken ct)
    {
        if (!TryParseDeviceType(deviceType, out var parsedDeviceType))
            return AuthResponseDto.Failure("Unsupported device type.");

        if (!ValidateMobileApiKey(parsedDeviceType, apiKey))
            return AuthResponseDto.Failure("Invalid mobile security key.");

        var storedToken = await tokenQueries.FetchFirstAsync(
            t => t.RefreshToken == request.RefreshToken && t.DeviceType == parsedDeviceType,
            ct);

        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            return AuthResponseDto.Failure("Invalid or expired refresh token.");

        var user = await userAuthQueries.GetByIdWithRoleAsync(storedToken.UserId, ct);
        if (user is null)
            return AuthResponseDto.Failure("User not found.");

        await tokenRepo.DeleteThisAsync(t => t.Id == storedToken.Id, ct);

        var newAccessToken = jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = Guid.NewGuid().ToString("N");
        var (tokenExpiry, refreshTokenExpiry) = GetExpiryWindows(parsedDeviceType);

        var newTokenEntity = CreateTokenEntity(
            user.Id,
            user.TenantId,
            parsedDeviceType,
            newAccessToken,
            newRefreshToken,
            tokenExpiry,
            refreshTokenExpiry);

        await tokenRepo.SaveMeAsync(newTokenEntity, ct);
        return AuthResponseDto.SuccessLogin(newAccessToken, newRefreshToken, user);
    }

    private static bool TryParseDeviceType(string deviceType, out DeviceType parsedDeviceType)
        => Enum.TryParse(deviceType, true, out parsedDeviceType);

    private async Task<User?> GetAndValidateUserCredentialsAsync(LoginRequestDto request, CancellationToken ct)
    {
        var user = await userAuthQueries.GetByEmailWithRoleAsync(request.Email, ct);
        if (user is null)
            return null;

        return user.VerifyPassword(request.Password) ? user : null;
    }

    private bool ValidateMobileApiKey(DeviceType deviceType, string? apiKey)
    {
        var isMobile = deviceType == DeviceType.MOBILE;
        if (!isMobile)
            return true;

        var validKey = _config["ApiSettings:MobileApiKey"];
        return !string.IsNullOrEmpty(apiKey) && apiKey == validKey;
    }

    private Task RevokeOldDeviceTokenAsync(Guid userId, DeviceType deviceType, CancellationToken ct)
        => tokenRepo.DeleteThisAsync(t => t.UserId == userId && t.DeviceType == deviceType, ct);

    private static (DateTime TokenExpiry, DateTime RefreshTokenExpiry) GetExpiryWindows(DeviceType deviceType)
    {
        var isMobile = deviceType == DeviceType.MOBILE;
        var tokenExpiry = isMobile ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(24);
        var refreshTokenExpiry = isMobile ? DateTime.UtcNow.AddDays(35) : DateTime.UtcNow.AddDays(7);
        return (tokenExpiry, refreshTokenExpiry);
    }

    private static AccessAndRefreshToken CreateTokenEntity(
        Guid userId,
        Guid tenantId,
        DeviceType deviceType,
        string token,
        string refreshToken,
        DateTime tokenExpiry,
        DateTime refreshTokenExpiry)
    {
        return new AccessAndRefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            RefreshToken = refreshToken,
            UserId = userId,
            TokenExpiresAt = tokenExpiry,
            DeviceType = deviceType,
            RefreshTokenExpiresAt = refreshTokenExpiry,
            IsRevoked = false,
            TenantId = tenantId
        };
    }

    private static User CreateUserEntity(
            string Username,
            string Email,
            bool IsOwner,
            Guid TenantId,
            string PhoneNumber,
            Guid RoleId,
            Role Role,
            string Password = "" 
        )
    {
        return new User
        {
            Username = Username ,
            Email = Email,
            IsOwner = IsOwner,
            TenantId = TenantId,
            PhoneNumber = PhoneNumber,
            RoleId = RoleId,
            Role = Role,
            Password = Password
        };
    }

    private static Role CreateNewOwnerRole(Guid TenantId)
    =>new Role
    {
        Name = Role.COFOUNDERS.OWNER.ToString(),
        TenantId = TenantId
    };
}

public static class AuthServiceExtensions
{
    public static bool VerifyPassword(this User user, string password)
    {
        return new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, password) 
                == PasswordVerificationResult.Success;
    }
}
