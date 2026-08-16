using Infrastructure.Queries.QueryBuilderEngine;
namespace Api.Middlewares;
public class QueryEngineOptionsMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, QueryOptions queryOptions)
    {
        HandlePagination(context, queryOptions);
        HandleSearch(context, queryOptions);
        HandleFilter(context, queryOptions);
        HandleSorting(context, queryOptions);
        await _next(context);
    }

    // ?sort=key&sort-direction=asc|desc
    private void HandleSorting(HttpContext context, QueryOptions queryOptions)
    {
        if (context.Request.Query.TryGetValue("sort", out var sort))
            queryOptions.Sort.Sort = sort;
            
        if (context.Request.Query.TryGetValue("sort-direction", out var sortDirection))
            queryOptions.Sort.SortDirection = sortDirection;
    }

    // ?filter=key1:value1,key2:value2,key3:value3,etc
    private void HandleFilter(HttpContext context, QueryOptions queryOptions)
    {
        if(context.Request.Query.TryGetValue("filter", out var filter))
        {
            var filters = filter.ToString().Split(',');
            foreach (var f in filters)
            {
                var keyValue = f.Split(':');
                if (keyValue.Length == 2)
                    queryOptions.Filters[keyValue[0]] = keyValue[1];
            }
        }
    }

    // ?search=searchTerm
    private void HandleSearch(HttpContext context, QueryOptions queryOptions)
    {
        if (context.Request.Query.TryGetValue("search", out var search))
            queryOptions.Search = search;
    }
    
    // ?page=1&pageSize=10
    private void HandlePagination(HttpContext context, QueryOptions queryOptions)
    {
        int.TryParse(
            context.Request.Query["pageSize"],
            out var parsedSize
        );
        var size = parsedSize <= 0 ? 10 : Math.Min(parsedSize, 300);

        int.TryParse(
            context.Request.Query["page"],
            out var parsedPage
        );
        var page = parsedPage <= 0 ? 1 : parsedPage;

        queryOptions.Pagination = new PaginationOptions
        {
            Page = page,
            Size = size
        };
    }
}

public static class QueryOptionsMiddlewareExtensions
{
    public static IApplicationBuilder UseQueryEngine(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<QueryEngineOptionsMiddleware>();
    }
}
