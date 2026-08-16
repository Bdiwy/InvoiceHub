using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Interfaces.Queries;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Queries.QueryBuilderEngine;
namespace Infrastructure.Queries
{
    public class CommonQueries<TEntity>(ApplicationDbContext context, QueryEngineCore<TEntity> queryEngine) : ICommonQueries<TEntity>
    where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
        private readonly QueryEngineCore<TEntity> _queryEngine = queryEngine;

        public async Task<TEntity?> FetchFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<TEntity?> GetEntityByIdAsync(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }
        
        public async Task<List<TEntity>> GetAllByQueryEngineAsync()
        {
            return await _queryEngine.Handle();
        }

        public async Task<List<TEntity>> GetAllEntitiesAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TEntity>> GetEntitiesDataWithConditionAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await _dbSet.AsNoTracking().Where(condition).ToListAsync();
        }

        public async Task<bool> CheckExistencData(Expression<Func<TEntity, bool>> condition, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().AnyAsync(condition, ct);
        }

    }
}