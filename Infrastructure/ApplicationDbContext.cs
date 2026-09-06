using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class ApplicationDbContext : DbContext
{
    private readonly Guid? _currentTenantId;
    public Guid? CurrentTenantId => _currentTenantId;
    public ILogedInUserData _currentUser { get; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService, ILogedInUserData logedInUserData) : base(options)
    {
        _currentUser = logedInUserData;
        _currentTenantId = tenantService.GetTenantId();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ApplyQueryFilters(modelBuilder);
    }

    private void ApplyQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            //Apply Tenant Query Filters
            var method = typeof(ApplicationDbContext)
                .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [modelBuilder]);

            if (!typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            //Apply Deleted Query Filters
            var deletedMethod = typeof(ApplicationDbContext)
                .GetMethod(nameof(SetDeletedFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);

            deletedMethod.Invoke(this, [modelBuilder]);
        }
    }

    private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity =>
                CurrentTenantId == null || entity.TenantId == CurrentTenantId);
    }

    private void SetDeletedFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IAuditableEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => entity.DeletedAt == null);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is not User && entry.Entity is IAuditableEntity auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.CreatedById = _currentUser.UserId;
                }

                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = DateTime.UtcNow;
                    auditable.UpdatedById = _currentUser.UserId;

                }
                else if(entry.State == EntityState.Deleted)
                {
                    auditable.DeletedAt = DateTime.UtcNow;
                    auditable.DeletedById = _currentUser.UserId;

                    entry.State = EntityState.Modified;

                }
            }

            if (entry.Entity is ITenantEntity tenant && entry.State == EntityState.Added && entry.Entity is not User)
                tenant.TenantId = _currentUser.TenantId;

        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}