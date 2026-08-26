using System.Linq.Expressions;

namespace Domain.QueryEngineConfigs.core;

public sealed class QueryConfig<TEntity>
{
    public List<string> SearchFields { get; } = [];
    public List<string> FilterFields { get; } = [];
    public List<string> SortFields { get; } = [];

    /// <summary>
    /// Registers a property as an allowed search field.
    /// </summary>
    public void Search<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        SearchFields.Add(GetPropertyName(expression));
    }
     
    /// <summary>
    /// Registers a property as an allowed filter field.
    /// </summary>
    public void Filter<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        FilterFields.Add(GetPropertyName(expression));
    }

    /// <summary>
    /// Registers a property as an allowed sort field.
    /// </summary>
    public void Sort<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        SortFields.Add(GetPropertyName(expression));
    }

    private static string GetPropertyName<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException(
            "Expression must be a property expression.");
    }
}