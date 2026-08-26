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
    /// <summary>
    /// Provides common query operations for entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    public class CommonQueries<TEntity>(ApplicationDbContext context, QueryEngineCore<TEntity> queryEngine) : ICommonQueries<TEntity>
    where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
        private readonly QueryEngineCore<TEntity> _queryEngine = queryEngine;

        /// <summary>
        /// Returns the first entity matching the predicate with no tracking, or null if none is found.
        /// </summary>
        public async Task<TEntity?> FetchFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        /// <summary>
        /// Returns the entity with the given Id, or null if it does not exist with no tracking.
        /// </summary>
        public async Task<TEntity?> GetEntityByIdAsync(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }
        
        /// <summary>
        /// Returns entities using the query engine (validation, filter, search, sort, and pagination).
        /// </summary>
        public async Task<List<TEntity>> GetAllByQueryEngineAsync()
        {
            return await _queryEngine.Handle();
        }

        /// <summary>
        /// Returns all entities without tracking.
        /// </summary>
        public async Task<List<TEntity>> GetAllEntitiesAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns entities that match the given condition without tracking.
        /// </summary>
        public async Task<List<TEntity>> GetEntitiesDataWithConditionAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await _dbSet.AsNoTracking().Where(condition).ToListAsync();
        }

        /// <summary>
        /// Returns whether any entity matches the given condition.
        /// </summary>
        public async Task<bool> CheckExistencData(Expression<Func<TEntity, bool>> condition, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().AnyAsync(condition, ct);
        }

    }
}