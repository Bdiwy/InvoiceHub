using Application.Interfaces;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;

namespace InvoiceHub.Application.Handlers;

public record LogoutCommand(Guid UserId, string ApiKey, string DeviceType) : IRequest<AuthResponseDto>;

public class LogoutHandler(IAuthService authService)
    : IRequestHandler<LogoutCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LogoutCommand request, CancellationToken ct)
    {
        return await authService.LogoutAsync(request.UserId, request.ApiKey, request.DeviceType, ct);
    }
}
