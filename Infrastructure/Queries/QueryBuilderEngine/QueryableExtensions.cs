using Domain.QueryEngineConfigs.core;
using System.Linq.Expressions;

namespace Infrastructure.Queries.QueryBuilderEngine;

public static class QueryableEngineCoreExtensions
{
    /// <summary>
    /// Applies skip/take pagination to the query based on <see cref="QueryOptions.Pagination"/>.
    /// </summary>
    public static IQueryable<TEntity> ApplyPagination<TEntity>(
        this IQueryable<TEntity> query,
        QueryOptions options
    )
    {
        if (options.Pagination == null)
            return query;
        
        var take = options.Pagination.PageSize;
        var skip = (options.Pagination.Page - 1) * options.Pagination.PageSize;
     
        return query.Skip(skip).Take(take);
    }

    /// <summary>
    /// Applies equality filters to the query for each entry in <see cref="QueryOptions.Filters"/>.
    /// </summary>
    public static IQueryable<TEntity> ApplyFiltering<TEntity>(
        this IQueryable<TEntity> query,
        QueryOptions options)
    {
        if (options.Filters == null || !options.Filters.Any())
            return query;
        foreach (var filter in options.Filters)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.PropertyOrField(parameter, filter.Key);
            var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
            var convertedValue = Convert.ChangeType(filter.Value, targetType);
            var constant = Expression.Constant(convertedValue, property.Type);
            var comparison = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
            query = query.Where(lambda);
        }
        return query;
    }

    /// <summary>
    /// Applies a case-sensitive contains search across the entity's configured search fields.
    /// </summary>
    public static IQueryable<TEntity> ApplySearch<TEntity>(
    this IQueryable<TEntity> query,
    QueryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Search))
            return query;

        var config = QueryConfigurationRegistry.Get<TEntity>();
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var searchValue = Expression.Constant(options.Search, typeof(string));
        var nullConstant = Expression.Constant(null, typeof(string));

        Expression? searchExpression = null;

        foreach (var propertyName in config.SearchFields)
        {
            var propertyAccess = Expression.Property(parameter, propertyName);

            var isNotNull = Expression.NotEqual(propertyAccess, nullConstant);
            var containsCall = Expression.Call(propertyAccess, containsMethod!, searchValue);
            var safeContainsCall = Expression.AndAlso(isNotNull, containsCall);

            searchExpression = searchExpression == null
                ? safeContainsCall
                : Expression.OrElse(searchExpression, safeContainsCall);
        }

        var lambda = Expression.Lambda<Func<TEntity, bool>>(searchExpression!, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Applies ascending or descending ordering using <see cref="QueryOptions.Sort"/>.
    /// </summary>
    public static IQueryable<TEntity> ApplySorting<TEntity>(
        this IQueryable<TEntity> query,
        QueryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Sort.Sort))
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var property = Expression.PropertyOrField(
            parameter,
            options.Sort.Sort);

        var lambda = Expression.Lambda(
            property,
            parameter);

        if (string.Equals(
            options.Sort.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase))
        {
            return Queryable.OrderByDescending(
                query,
                (dynamic)lambda);
        }

        return Queryable.OrderBy(
            query,
            (dynamic)lambda);
    }
}
