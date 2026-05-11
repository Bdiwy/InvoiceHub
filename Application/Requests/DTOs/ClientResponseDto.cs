namespace InvoiceHub.Application.Requests.DTOs;

public record ClientResponseDto(
    Guid Id,
    string CompanyName,
    string ContactName, 
    string ContactEmail, 
    string ContactPhone,
    string ContactAddress,
    string TradeLicenseNumber
);