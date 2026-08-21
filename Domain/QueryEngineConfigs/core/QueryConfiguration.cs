namespace Domain.QueryEngineConfigs.core;

public abstract class QueryConfiguration<TEntity>
{
    public abstract void Configure(QueryConfig<TEntity> config);
}