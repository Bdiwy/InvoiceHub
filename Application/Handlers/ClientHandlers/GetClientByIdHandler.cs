using MediatR;
using Domain.Entities;
using InvoiceHub.Application.Requests.DTOs;
using Application.Interfaces.Queries;
namespace Application.Handlers.ClientHandlers;

public record GetClientByIdRequest(Guid ClientId) : IRequest<ClientResponseDto>;
public class GetClientByIdHandler(ICommonQueries<Client> clientRepo) : IRequestHandler<GetClientByIdRequest, ClientResponseDto?>
{
    public async Task<ClientResponseDto?> Handle(GetClientByIdRequest request, CancellationToken cancellationToken)
    {
        
        var client = await clientRepo.FetchFirstAsync(c => c.Id == request.ClientId, cancellationToken);
        if (client is null)
            return null;

        return new ClientResponseDto
        (
            client.Id,
            client.CompanyName,
            client.ContactName,
            client.ContactEmail,
            client.ContactPhone,
            client.ContactAddress,
            client.TradeLicenseNumber
        );
    }
}
