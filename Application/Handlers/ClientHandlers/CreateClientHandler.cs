using MediatR;
using Domain.Entities;
namespace InvoiceHub.Application.Handlers.ClientHandlers;
public record CreateClientHandler(
    string CompanyName,
    string ContactName, 
    string ContactEmail, 
    string ContactPhone,
    string ContactAddress,
    string TradeLicenseNumber
) : IRequest<ClientResponseDto>;
public class CreateClientHandler(ICommonCommands<Client> clientRepo) : IRequestHandler<CreateClientHandler, ClientResponseDto>
{
    public async Task<ClientResponseDto> Handle(CreateClientHandler request, CancellationToken cancellationToken)
    {
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            ContactAddress = request.ContactAddress,
            TradeLicenseNumber = request.TradeLicenseNumber
        };  

        await clientRepo.SaveMeAsync(newClient);

        return new ClientResponseDto(
            Id = newClient.Id,
            CompanyName = newClient.CompanyName,
            ContactName = newClient.ContactName,
            ContactEmail = newClient.ContactEmail,
            ContactPhone = newClient.ContactPhone,
            ContactAddress = newClient.ContactAddress,
            TradeLicenseNumber = newClient.TradeLicenseNumber
        );
    }
}