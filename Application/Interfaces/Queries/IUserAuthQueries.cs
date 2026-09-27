using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Queries;

public interface IUserAuthQueries
{
    /// <summary>
    /// Returns an IQueryable&lt;User&gt; that already has
    /// Role → RolePermissions → Permission Includes applied.
    /// Chain .Where / .FirstOrDefaultAsync / .ToListAsync on it — do not start a new DbSet query.
    /// </summary>
    IQueryable<User> Query();

    Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> FindThisAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
    Task<AccessAndRefreshToken?> GetStoredToken(Expression<Func<AccessAndRefreshToken, bool>> predicate, CancellationToken cancellationToken = default);
}
