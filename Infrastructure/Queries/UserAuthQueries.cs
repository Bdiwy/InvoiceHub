using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Queries;

public class UserAuthQueries(ApplicationDbContext context) : IScopedService, IUserAuthQueries
{
    private IQueryable<User> UsersWithAuthGraph()
        => context.Users
            .AsSplitQuery()
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission);

    public IQueryable<User> Query() => UsersWithAuthGraph();

    public Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
        => UsersWithAuthGraph()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken cancellationToken = default)
        => UsersWithAuthGraph()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> FindThisAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => UsersWithAuthGraph()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<AccessAndRefreshToken?> GetStoredToken(
        Expression<Func<AccessAndRefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default)
        => context.AccessAndRefreshTokens
            .AsSplitQuery()
            .FirstOrDefaultAsync(predicate, cancellationToken);
}
