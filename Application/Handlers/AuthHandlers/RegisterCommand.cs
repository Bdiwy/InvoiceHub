using Application.Interfaces;
using InvoiceHub.Application.Requests.DTOs;
using MediatR;
namespace Application.Handlers.AuthHandlers;

public record RegisterCommand(RegisterRequestDto RequestDto) : IRequest<AuthResponseDto>;
public class RegisterHandler(IAuthService authService) 
: IRequestHandler<RegisterCommand , AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request , CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request.RequestDto , ct);
        return result;
    }
} 