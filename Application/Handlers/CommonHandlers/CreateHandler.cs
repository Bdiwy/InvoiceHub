using Application.Handlers.ClientHandlers;
using Application.Interfaces.Queries;
using Domain.Entites;
using MediatR;

namespace InvoiceHub.Application.Handlers.CommonHandlers;

public record CreateRequest<TEntity>(TEntity Entity) : IRequest<ResponseDto<TEntity>>
    where TEntity : BaseDomainEntity;

public class CreateHandler<TEntity>(ICommonCommands<TEntity> entityRepo)
    : IRequestHandler<CreateRequest<TEntity>, ResponseDto<TEntity>>
    where TEntity : BaseDomainEntity
{
    public async Task<ResponseDto<TEntity>> Handle(
        CreateRequest<TEntity> request,
        CancellationToken cancellationToken)
    {
        await entityRepo.SaveMeAsync(request.Entity);
        return new ResponseDto<TEntity>(request.Entity);
    }
}
