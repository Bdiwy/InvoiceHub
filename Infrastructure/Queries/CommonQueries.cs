using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Interfaces.Queries;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class CommonQueries<T>(ApplicationDbContext context) : ICommonQueries<T>
    where T : class
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> FetchFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<T?> GetEntityByIdAsync(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<List<T>> GetAllEntitiesAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetEntitiesDataWithConditionAsync(Expression<Func<T, bool>> condition)
        {
            return await _dbSet.AsNoTracking().Where(condition).ToListAsync();
        }
    }
}