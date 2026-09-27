using Application.Interfaces.Queries;
using Domain.Entites;
using MediatR;

namespace Application.Handlers.CommonHandlers;

public record GetAll<TEntity>() : IRequest<IEnumerable<TEntity>>
    where TEntity : BaseDomainEntity;

public class GetAllHandler<TEntity>(ICommonQueries<TEntity> repo)
    : IRequestHandler<GetAll<TEntity>, IEnumerable<TEntity>>
    where TEntity : BaseDomainEntity
{
    public async Task<IEnumerable<TEntity>> Handle(GetAll<TEntity> request, CancellationToken cancellationToken)
    {
        return await repo.GetAllByQueryEngineAsync();
    }
}
