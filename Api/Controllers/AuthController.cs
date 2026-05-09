
using InvoiceHub.Application.Requests.DTOs;
using InvoiceHub.Application.Handlers;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace InvoiceHub.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator)  : ControllerBase
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request , CancellationToken ct)
    {
        string? apiKey = Request.Headers["X-Api-Key"].ToString();
        string deviceType = !string.IsNullOrEmpty(Request.Headers["X-Device-Type"]) 
                    ? Request.Headers["X-Device-Type"].ToString() 
                    : nameof(DeviceType.WEB);

        var result = await mediator.Send(new LoginCommand(request , apiKey , deviceType), ct);
        return Ok(result); 
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request , CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(request), ct);
        return Ok(result); 
    }

    [HttpPost("refresh-token") , AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        string? apiKey = Request.Headers["X-Api-Key"].ToString();
        string deviceType = !string.IsNullOrEmpty(Request.Headers["X-Device-Type"])
            ? Request.Headers["X-Device-Type"].ToString()
            : nameof(DeviceType.WEB);

        var result = await mediator.Send(new RefreshTokenCommand(request, apiKey, deviceType), ct);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<AuthResponseDto>> LogOut(CancellationToken ct)
    {
        string? apiKey = Request.Headers["X-Api-Key"].ToString();
        string deviceType = !string.IsNullOrEmpty(Request.Headers["X-Device-Type"])
            ? Request.Headers["X-Device-Type"].ToString()
            : nameof(DeviceType.WEB);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(AuthResponseDto.Failure("Invalid token subject."));

        var result = await mediator.Send(new LogoutCommand(userId, apiKey, deviceType), ct);
        return Ok(result);
    }
}