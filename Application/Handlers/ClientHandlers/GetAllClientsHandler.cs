using MediatR;
using Domain.Entities;
using InvoiceHub.Application.Requests.DTOs;
using Application.Interfaces.Queries;
namespace Application.Handlers.ClientHandlers;

public record GetAllClientsRequest() : IRequest<IEnumerable<ClientsResponseDto>>;
public class GetAllClientsHandler(ICommonQueries<Client> clientRepo) : IRequestHandler<GetAllClientsRequest, IEnumerable<ClientsResponseDto>>
{
    public async Task<IEnumerable<ClientsResponseDto>> Handle(GetAllClientsRequest request, CancellationToken cancellationToken)
    {
        var clients = await clientRepo.GetAllEntitiesAsync();
        return clients.Select(client => new ClientsResponseDto(
            client.Id,
            client.CompanyName,
            client.ContactEmail,
            client.TradeLicenseNumber
        ));
    }
}