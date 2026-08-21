using Domain.Interfaces;

namespace Infrastructure.Queries.QueryBuilderEngine;
public sealed record QueryOptions: IRegisterAsSelf
{
    public PaginationOptions Pagination { get; set; } = new();
    public Dictionary<string, string> Filters { get; set; } = new();
    public string? Search { get; set; }
    public SortOptions Sort { get; set; } = new();
}

public sealed record PaginationOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; } = 0;
    public int TotalNumberOfPages { get; set; } = 0;
}

public sealed record SortOptions
{
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
}
