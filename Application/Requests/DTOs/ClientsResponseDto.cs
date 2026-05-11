namespace InvoiceHub.Application.Requests.DTOs;

public record ClientsResponseDto(
    Guid Id,
    string CompanyName,
    string ContactEmail, 
    string TradeLicenseNumber
);