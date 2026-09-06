using Application.Configs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Application.Configs;
using Domain.Interfaces;
namespace Infrastructure.Seed;

public class DatabaseSeeder(ApplicationDbContext context, ILogedInUserData currentUser, IOptions<SystemUserSettings> systemUserOptions): IRegisterAsSelf
{
    public async Task SeedAsync()
    {
        await SeedSystemUserAndOwnerRoleAsync();
        //await SeedClients();
    }

    private async Task SeedSystemUserAndOwnerRoleAsync()
    {
        string ownerRoleName = Role.COFOUNDERS.OWNER.ToString();
        bool userExists = await context.Users.AnyAsync(u => u.Username == systemUserOptions.Value.Username || u.Email == systemUserOptions.Value.Email);
        bool roleExists = await context.Roles.AnyAsync(r => r.Name == ownerRoleName);

        if (!userExists && !roleExists)
        {

            var systemUser = new User
            {
                Username = systemUserOptions.Value.Username,
                Email = systemUserOptions.Value.Email,
                PhoneNumber = systemUserOptions.Value.PhoneNumber,
                Password = systemUserOptions.Value.Password,
                IsOwner = true,
                RoleId = null,
                CreatedAt = DateTime.UtcNow,
                TenantId = Guid.NewGuid()
            };

            context.Users.Add(systemUser);
            await context.SaveChangesAsync();
            currentUser.UseSystemUser(systemUser.Id, systemUser.TenantId);

            var ownerRole = new Role { Name = ownerRoleName };
            context.Roles.Add(ownerRole);
            await context.SaveChangesAsync();

            systemUser.RoleId = ownerRole.Id;
            await context.SaveChangesAsync();

            await AssignAllPermissionsToAsync(ownerRole.Id);
        }
    }

    private async Task AssignAllPermissionsToAsync(Guid roleId)
    {
        var allPermissions = await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            bool exists = await context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (!exists)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedClients()
    {
        currentUser.UseSystemUser();
        var existingCount = await context.Clients
            .CountAsync(c => c.TenantId == currentUser.TenantId);

        var clients = new List<Client>();
        var random = new Random();
        for (int i = existingCount + 1; i <= 20000; i++)
        {
            clients.Add(new Client
            {
                CompanyName = $"Company {i}",
                ContactName = $"Contact Person {random.Next(1000, 99999)}",
                ContactEmail = $"contact{random.Next(1000, 99999)}@company.com",
                ContactPhone = $"+20 100 000 {i:D4}",
                ContactAddress = $"Business Address {i}, Cairo, Egypt",
                TradeLicenseNumber = $"TL-2026-{i:D4}"
            });
        }

        if (clients.Count == 0)
            return;

        await context.Clients.AddRangeAsync(clients);
        await context.SaveChangesAsync();
    }

}