using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Queries.QueryBuilderEngine;


public class QueryEngineCore<TEntity> : IRegisterAsSelf
    where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly QueryOptions _options;

    /// <summary>
    /// Creates a query engine for <typeparamref name="TEntity"/> using the given context and options.
    /// </summary>
    public QueryEngineCore(ApplicationDbContext context, QueryOptions options)
    {  
        _context = context;
        _options = options;
    }

    /// <summary>
    /// Builds, validates, filters, searches, sorts, and paginates the entity set, then returns the page results.
    /// </summary>
    public async Task<List<TEntity>> Handle()
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        query = query
                     .ApplyValidationConfigration(_options)
                     .ApplyFiltering(_options)
                     .ApplySearch(_options)
                     .ApplySorting(_options);
                            

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)_options.Pagination.PageSize
            );

        _options.Pagination.TotalCount = totalCount;
        _options.Pagination.TotalNumberOfPages = totalPages;

        var finalQuery = query.ApplyPagination(_options);

        return await finalQuery.ToListAsync();
    }


}
