using System.Linq.Expressions;

namespace Domain.QueryEngineConfigs.core;

public sealed class QueryConfig<TEntity>
{
    public List<string> SearchFields { get; } = [];
    public List<string> FilterFields { get; } = [];
    public List<string> SortFields { get; } = [];

    public void Search<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        SearchFields.Add(GetPropertyName(expression));
    }
     
    public void Filter<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        FilterFields.Add(GetPropertyName(expression));
    }

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