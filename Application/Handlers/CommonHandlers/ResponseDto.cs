using Domain.Entites;

namespace Application.Handlers.ClientHandlers;
    public class ResponseDto<TEntity>
        where TEntity : BaseDomainEntity
    {

    public ResponseDto(Dictionary<string, object?> data)
    {
        Data = data;
    }
    
    public ResponseDto(TEntity entity)
    {
        Data = typeof(TEntity)
            .GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(IncludeInResponseAttribute)))
            .ToDictionary(
                p => p.Name,
                p => p.GetValue(entity)
            );
    }
    public Dictionary<string, object?> Data { get; }
}