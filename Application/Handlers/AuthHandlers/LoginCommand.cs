using Application.Interfaces;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;
namespace Application.Handlers.AuthHandlers;

public record LoginCommand(LoginRequestDto RequestDto, string? apiKey, string deviceType) : IRequest<AuthResponseDto>;
public class LoginRequestHandler(IAuthService authService) 
: IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request , CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.RequestDto, request.apiKey, request.deviceType , ct);
        return result; 
    }
    
}