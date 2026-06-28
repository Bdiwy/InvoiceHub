using Domain.Entities;

namespace InvoiceHub.Application.Requests.DTOs;
public record AuthResponseDto(
    bool IsSuccess,
    string? Message = null,
    string? Token = null,
    string? RefreshToken = null,
    string? Username = null,
    string? Email = null,
    bool? IsOwner = null,
    Guid? TenantId = null)
{

// Factory method for success
public static AuthResponseDto Success(string token,string refreshToken, User user , string message = null) => new(
    IsSuccess: true,
    Message: message,
    Token: token,
    Username: user.Username,
    Email: user.Email,
    IsOwner: user.IsOwner,
    TenantId: user.TenantId,
    RefreshToken:refreshToken
);

public static AuthResponseDto SuccessLogin(string token,string refreshToken, User user) => new(
    IsSuccess: true,
    Message: "Login successful.",
    Token: token,
    Username: user.Username,
    Email: user.Email,
    IsOwner: user.IsOwner,
    TenantId: user.TenantId,
    RefreshToken:refreshToken
);

// Factory method for failure
public static AuthResponseDto Failure(string message) => new(
    IsSuccess: false,
    Message: message
);


// register method for success 
public static AuthResponseDto SuccessRegister() => new(
    IsSuccess: true,
    Message: "Registration successful."
);

public static AuthResponseDto SuccessLogout() => new(
    IsSuccess: true,
    Message: "Logout successful."
);

}