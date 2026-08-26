using System.Linq.Expressions;

namespace Application.Interfaces.Queries
{
    /// <summary>
    /// Defines shared read-only query operations for entities of type <typeparamref name="T"/>.
    /// Implementations typically query without change tracking.
    /// </summary>
    /// <typeparam name="T">The entity type to query. Must be a reference type.</typeparam>
    public interface ICommonQueries<T> where T : class
    {
        /// <summary>
        /// Retrieves the first entity that satisfies the given predicate.
        /// </summary>
        /// <param name="predicate">A LINQ expression that selects matching entities.</param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The first matching entity, or <c>null</c> when no entity matches the predicate.
        /// </returns>
        public Task<T?> FetchFirstAsync(Expression<Func<T, bool>> predicate , CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves every entity of type <typeparamref name="T"/> from the data store.
        /// </summary>
        /// <returns>A list containing all entities. The list is empty when none exist.</returns>
        public Task<List<T>> GetAllEntitiesAsync();

        /// <summary>
        /// Retrieves entities through the query engine, applying validation, filtering, search, sorting, and pagination
        /// from the current request query options.
        /// </summary>
        /// <returns>
        /// A list of entities for the requested page after query-engine processing.
        /// Pagination totals are updated on the shared query options.
        /// </returns>
        public Task<List<T>> GetAllByQueryEngineAsync();

        /// <summary>
        /// Retrieves all entities that satisfy the given condition.
        /// </summary>
        /// <param name="condition">A LINQ expression that filters which entities are returned.</param>
        /// <returns>A list of matching entities. The list is empty when none match.</returns>
        public Task<List<T>> GetEntitiesDataWithConditionAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Checks whether at least one entity satisfies the given condition.
        /// </summary>
        /// <param name="condition">A LINQ expression that defines the existence check.</param>
        /// <param name="ct">Token used to cancel the asynchronous operation.</param>
        /// <returns><c>true</c> if at least one matching entity exists; otherwise <c>false</c>.</returns>
        public Task<bool> CheckExistencData(Expression<Func<T, bool>> condition, CancellationToken ct = default);
    }
}
