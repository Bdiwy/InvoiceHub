using InvoiceHub.Application.Requests.DTOs;
using InvoiceHub.Application.Requests;
namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? apiKey, string deviceType, CancellationToken ct);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct);
        Task<AuthResponseDto> LogoutAsync(Guid userId, string? apiKey, string deviceType, CancellationToken ct);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? apiKey, string deviceType, CancellationToken ct);
    }
}