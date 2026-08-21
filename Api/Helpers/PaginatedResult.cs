using Domain.Entities;
using Infrastructure.Queries.QueryBuilderEngine;
namespace Api.Helpers;

public class PaginatedResult<TEntity>
{
    private readonly QueryOptions _options;
    public PaginatedResult(IEnumerable<TEntity> items, QueryOptions options)
    {
        Items = items;
        _options = options;
        CurrentPage = _options.Pagination.Page;
        TotalPages = _options.Pagination.TotalNumberOfPages;
        PageSize = _options.Pagination.PageSize;
        TotalCount = _options.Pagination.TotalCount;
    }

    public IEnumerable<TEntity> Items { get; set; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;


}