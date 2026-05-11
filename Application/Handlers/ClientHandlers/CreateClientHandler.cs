using MediatR;
using Domain.Entities;
using InvoiceHub.Application.Requests.DTOs;
using Application.Interfaces.Queries;
namespace InvoiceHub.Application.Handlers.ClientHandlers;
public record CreateClientRequest(
    string CompanyName,
    string ContactName, 
    string ContactEmail, 
    string ContactPhone,
    string ContactAddress,
    string TradeLicenseNumber
) : IRequest<ClientResponseDto>;
public class CreateClientHandler(ICommonCommands<Client> clientRepo) : IRequestHandler<CreateClientRequest, ClientResponseDto>
{
    public async Task<ClientResponseDto> Handle(CreateClientRequest request, CancellationToken cancellationToken)
    {
        Client newClient = new Client
        {
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            ContactAddress = request.ContactAddress,
            TradeLicenseNumber = request.TradeLicenseNumber,

        };  

        await clientRepo.SaveMeAsync(newClient);

        return new ClientResponseDto(
            newClient.Id,
            newClient.CompanyName,
            newClient.ContactName,
            newClient.ContactEmail,
            newClient.ContactPhone,
            newClient.ContactAddress,
            newClient.TradeLicenseNumber
        );
    }
}