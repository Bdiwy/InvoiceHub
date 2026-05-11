using MediatR;
using Domain.Entities;
namespace Application.Handlers.ClientHandlers;

public record GetAllClientsHandler() : IRequest<IEnumerable<ClientsResponseDto>>;
public class GetAllClientsHandler(ICommonQueries<Client> clientRepo) : IRequestHandler<GetAllClientsHandler, IEnumerable<ClientsResponseDto>>
{
    public async Task<IEnumerable<ClientsResponseDto>> Handle(GetAllClientsHandler request, CancellationToken cancellationToken)
    {
        var clients = await clientRepo.GetAllEntitiesAsync();
        return clients.Select(client => new ClientsResponseDto(
            Id = client.Id,
            CompanyName = client.CompanyName,
            ContactEmail = client.ContactEmail,
            TradeLicenseNumber = client.TradeLicenseNumber
        ));
    }
}