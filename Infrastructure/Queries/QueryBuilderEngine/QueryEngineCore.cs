
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries.QueryBuilderEngine;


public class QueryEngineCore<TEntity> : IRegisterAsSelf
    where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly QueryOptions options;

    public QueryEngineCore(ApplicationDbContext context, QueryOptions options)
    {
        _context = context;
        this.options = options; // populated by middleware
    }

    public async Task<List<TEntity>> Handle()
    {
        var x = options.Pagination.Size ;
        IQueryable<TEntity> query = _context.Set<TEntity>();
        var result = await _context.Set<TEntity>().ToListAsync();
        return result;
    }

}
