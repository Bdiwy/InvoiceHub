using Application.Exceptions;
using Domain.Entities;
using Domain.QueryEngineConfigs;
using Domain.QueryEngineConfigs.core;
using System.Linq.Expressions;
namespace Infrastructure.Queries.QueryBuilderEngine;

public static class QueryableEngineValidationExtensions
{
    public static IQueryable<TEntity> ApplyValidationConfigration<TEntity>(
        this IQueryable<TEntity> query,
        QueryOptions options
    )
    {
        var config = QueryConfigurationRegistry.Get<TEntity>();
        CheckConfigForSearch<TEntity>(options, config);
        CheckConfigForFilter<TEntity>(options, config);
        CheckConfigForSort<TEntity>(options, config);
        return query;
    }

    private static void CheckConfigForSort<TEntity>(QueryOptions options, QueryConfig<TEntity> config)
    {
        if (options.Sort != null && !string.IsNullOrWhiteSpace(options.Sort.Sort))
        {
            if (!config.SortFields.Contains(options.Sort.Sort))
            {
                throw new AppValidationException(
                    $"Sorting by '{options.Sort.Sort}' is not allowed for {typeof(TEntity).Name}."
                );
            }
        }
    }

    private static void CheckConfigForFilter<TEntity>(QueryOptions options, QueryConfig<TEntity> config)
    {
        if (options.Filters is { Count: > 0 })
        {
            // validate keys/values are not empty
            if (options.Filters.Any(kv => string.IsNullOrWhiteSpace(kv.Key)
                                       || string.IsNullOrWhiteSpace(kv.Value)))
            {
                throw new AppValidationException(
                    $"Filters contain empty key or value, not allowed for {typeof(TEntity).Name}."
                );
            }

            // validate keys are allowed
            foreach (var kv in options.Filters)
            {
                if (!config.FilterFields.Contains(kv.Key))
                {
                    throw new AppValidationException(
                        $"Filtering by '{kv.Key}' is not allowed for {typeof(TEntity).Name}."
                    );
                }
            }
        }
    }

    private static void CheckConfigForSearch<TEntity>(QueryOptions options, QueryConfig<TEntity> config)
    {
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            if (!config.SearchFields.Any())
            {
                throw new AppValidationException(
                    $"Searching is not allowed for {typeof(TEntity).Name}."
                );
            }
        }
    }

}