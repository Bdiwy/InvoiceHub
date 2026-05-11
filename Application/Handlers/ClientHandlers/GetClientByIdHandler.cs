using MediatR;
using Domain.Entities;
namespace Application.Handlers.ClientHandlers;

public record GetClientByIdHandler(Guid ClientId) : IRequest<ClientResponseDto>;
public class GetClientByIdHandler(ICommonQueries<Client> clientRepo) : IRequestHandler<GetClientByIdHandler, ClientResponseDto>
{
    public async Task<ClientResponseDto?> Handle(GetClientByIdHandler request, CancellationToken cancellationToken)
    {
        
        var client = await clientRepo.FetchFirstAsync(c => c.Id == request.ClientId, cancellationToken);
        if (client is null)
            return null;

        return new ClientResponseDto
        {
            Id = client.Id,
            ContractName = client.ContactName,
            ContractEmail = client.ContactEmail,
            ContractPhoneNumber = client.ContactPhoneNumber,
            ContractAddress = client.ContactAddress,
            TradeLicenseNumber = client.TradeLicenseNumber
        };
    }
}
