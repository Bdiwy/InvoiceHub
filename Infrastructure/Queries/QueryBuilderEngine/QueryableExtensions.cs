using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Queries.QueryBuilderEngine;

public static class QueryableEngineCoreExtensions
{
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
            var constant = Expression.Constant(filter.Value);
            var comparison = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
            query = query.Where(lambda);
        }
        return query;
    }

    public static IQueryable<TEntity> ApplySearch<TEntity>(
        this IQueryable<TEntity> query,
        QueryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Search))
            return query;
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.PropertyType == typeof(string));
        Expression? searchExpression = null;
        foreach (var property in properties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var searchValue = Expression.Constant(options.Search, typeof(string));
            var containsCall = Expression.Call(propertyAccess, containsMethod!, searchValue);
            searchExpression = searchExpression == null
                ? containsCall
                : Expression.OrElse(searchExpression, containsCall);
        }
        if (searchExpression == null)
            return query;
        var lambda = Expression.Lambda<Func<TEntity, bool>>(searchExpression, parameter);
        return query.Where(lambda);
    }

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
