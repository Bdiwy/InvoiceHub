using Application.Interfaces;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;

namespace Application.Handlers.AuthHandlers;

public record RefreshTokenCommand(RefreshTokenRequestDto RequestDto, string? ApiKey, string DeviceType) : IRequest<AuthResponseDto>;

public class RefreshTokenHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        return await authService.RefreshTokenAsync(request.RequestDto, request.ApiKey, request.DeviceType, ct);
    }
}
