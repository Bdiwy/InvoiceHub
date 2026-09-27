using Application.Handlers.ClientHandlers;
using Application.Interfaces.Queries;
using Domain.Entites;
using MediatR;

namespace Application.Handlers.CommonHandlers;

public record GetTEntityByIdRequest<TEntity>(Guid Id) : IRequest<ResponseDto<TEntity>?>
    where TEntity : BaseDomainEntity;

public class GetTEntityByIdHandler<TEntity>(ICommonQueries<TEntity> repo)
    : IRequestHandler<GetTEntityByIdRequest<TEntity>, ResponseDto<TEntity>?>
    where TEntity : BaseDomainEntity
{
    public async Task<ResponseDto<TEntity>?> Handle(
        GetTEntityByIdRequest<TEntity> request,
        CancellationToken cancellationToken)
    {
        var entity = await repo.GetEntityByIdAsync(c => c.Id == request.Id, cancellationToken);
        if (entity is null)
            return null;

        return new ResponseDto<TEntity>(entity);
    }
}
