using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface ICommonQueries<T> where T : class
    {
        public Task<T?> FetchFirstAsync(Expression<Func<T, bool>> predicate , CancellationToken cancellationToken = default);
        public Task<List<T>> GetAllEntitiesAsync();
        public Task<List<T>> GetAllByQueryEngineAsync();
        public Task<List<T>> GetEntitiesDataWithConditionAsync(Expression<Func<T, bool>> condition);
        public Task<bool> CheckExistencData(Expression<Func<T, bool>> condition, CancellationToken ct = default);
    }
}