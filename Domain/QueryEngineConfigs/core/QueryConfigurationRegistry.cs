using Domain.Entities;

namespace Domain.QueryEngineConfigs.core;

public static class QueryConfigurationRegistry
{
    private static readonly Dictionary<Type, object> _configs = new();

    /// <summary>
    /// Initializes the QueryConfigurationRegistry with configurations for different entities.
    /// this is a static constructor that registers configurations for various entities in the application. (like Di)
    /// </summary>
    static QueryConfigurationRegistry()
    {
        _configs[typeof(Client)] = new ClientQueryEngineConfiguration();
        _configs[typeof(Invoice)] = new InvoiceQueryEngineConfiguration();
        // register more entities here
    }

    public static QueryConfig<TEntity> Get<TEntity>()
    {
        var configuration = (QueryConfiguration<TEntity>)_configs[typeof(TEntity)];
        var config = new QueryConfig<TEntity>();
        configuration.Configure(config);
        return config;
    }
}
