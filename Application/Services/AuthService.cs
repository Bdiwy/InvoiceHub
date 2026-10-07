using Application.Exceptions;
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
    IUserAuthQueries userRepo,
    ICommonQueries<Role> roleQueries,
    ICommonCommands<AccessAndRefreshToken> tokenRepo,
    ICommonCommands<User> userCommandsRepo,
    IConfiguration _config) : IScopedService , IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? apiKey, string deviceType, CancellationToken ct)
    {
        var parsedDeviceType = ParseDeviceType(deviceType);
        var user = await GetAndValidateUserCredentialsAsync(request, ct);

        ValidateMobileApiKey(parsedDeviceType, apiKey);

        await RevokeOldDeviceTokenAsync(user.Id, parsedDeviceType, ct);

        var token = jwtTokenGenerator.GenerateToken(user);
        var refreshToken = Guid.NewGuid().ToString("N");
        var (tokenExpiry, refreshTokenExpiry) = GetExpiryWindows(parsedDeviceType);

        var tokenRegistration = new AccessAndRefreshToken(user.Id, user.TenantId, parsedDeviceType, token, refreshToken, tokenExpiry, refreshTokenExpiry);
        await tokenRepo.SaveMeAsync(tokenRegistration, ct);

        return AuthResponseDto.SuccessLogin(token, refreshToken, user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var existingUser = await userRepo.FindThisAsync(u => u.Email == request.Email || u.Username == request.Username || u.PhoneNumber == request.PhoneNumber, ct);
        if (existingUser is not null)
        {
            if (string.Equals(existingUser.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                throw new AppValidationException("Email already in use");

            if (string.Equals(existingUser.Username, request.Username, StringComparison.OrdinalIgnoreCase))
                throw new AppValidationException("Username already in use");

            if (existingUser.PhoneNumber == request.PhoneNumber)
                throw new AppValidationException("PhoneNumber is already registered");
        }

        var newTenantId = Guid.NewGuid();
        var ownerRole = await roleQueries.FetchFirstAsync(e=> e.Name  == Role.COFOUNDERS.OWNER.ToString());

        var newUser = new User(newTenantId, ownerRole!.Id, request.Username, request.Email, true, request.PhoneNumber);
        newUser.Password = new PasswordHasher<User>().HashPassword(newUser, request.Password);

        await userCommandsRepo.SaveMeAsync(newUser, ct);
        return AuthResponseDto.SuccessRegister();
    }

    public async Task<AuthResponseDto> LogoutAsync(Guid userId, string? apiKey, string deviceType, CancellationToken ct)
    {
        var parsedDeviceType = ParseDeviceType(deviceType);

        ValidateMobileApiKey(parsedDeviceType, apiKey);

        await RevokeOldDeviceTokenAsync(userId, parsedDeviceType, ct);
        return AuthResponseDto.SuccessLogout();
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? apiKey, string deviceType, CancellationToken ct)
    {
        var parsedDeviceType = ParseDeviceType(deviceType);

        ValidateMobileApiKey(parsedDeviceType, apiKey);

        var storedToken = await userRepo.GetStoredToken(
            t => t.RefreshToken == request.RefreshToken && t.DeviceType == parsedDeviceType,
            ct);

        if (storedToken is null || storedToken.IsRevoked || storedToken.IsExpired)
            return AuthResponseDto.Failure("Invalid or expired refresh token");

        var user = await userAuthQueries.GetByIdWithRoleAsync(storedToken.UserId, ct);
        if (user is null)
            return AuthResponseDto.Failure("User not found");

        await tokenRepo.DeleteThisAsync(t => t.Id == storedToken.Id, ct);

        var newAccessToken = jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = Guid.NewGuid().ToString("N");
        var (tokenExpiry, refreshTokenExpiry) = GetExpiryWindows(parsedDeviceType);

        var newTokenEntity = new AccessAndRefreshToken(
            user.Id,
            user.TenantId,
            parsedDeviceType,
            newAccessToken,
            newRefreshToken,
            tokenExpiry,
            refreshTokenExpiry);

        await tokenRepo.SaveMeAsync(newTokenEntity, ct);
        return AuthResponseDto.Success(newAccessToken, newRefreshToken, user);
    }

    private static DeviceType ParseDeviceType(string deviceType)
    {
        if (!Enum.TryParse(deviceType, true, out DeviceType parsedDeviceType))
            throw new InternalServerErrorException("Unsupported device type");
        return parsedDeviceType;
    }


    private async Task<User?> GetAndValidateUserCredentialsAsync(LoginRequestDto request, CancellationToken ct)
    {
        var user = await userAuthQueries.GetByEmailWithRoleAsync(request.Email, ct);
        user = user.VerifyPassword(request.Password) ? user : null;
        if (user is null)
            throw new AppValidationException("Invalid email or password");
        
        return user;
    }

    private void ValidateMobileApiKey(DeviceType deviceType, string? apiKey)
    {
        var isMobile = deviceType == DeviceType.MOBILE;
        if (!isMobile)
            return;

        var validKey = _config["ApiSettings:MobileApiKey"];
        if (string.IsNullOrEmpty(apiKey) || apiKey != validKey)
        {
            throw new InternalServerErrorException("Invalid mobile security key");
        }
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
}

public static class AuthServiceExtensions
{
    public static bool VerifyPassword(this User user, string password)
    {
        return new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, password) 
                == PasswordVerificationResult.Success;
    }
}
