using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed;
public class DatabaseSeeder(ApplicationDbContext context)
{
    public Task SeedAsync() => Task.CompletedTask;
    //public async Task SeedAsync()
    //{
    //    await SeedClients();
    //}

    private async Task SeedOwnerRole()
    {
        // Check if the Owner role already exists
        string ownerRoleName = Role.COFOUNDERS.OWNER.ToString();
        var systemTenantId = Guid.Empty;
        if (!await context.Roles.AnyAsync(r => r.Name == ownerRoleName && r.TenantId == systemTenantId))
        {
            var ownerRole = new Role 
            { 
                Name = ownerRoleName,
                TenantId = systemTenantId
            };
            context.Roles.Add(ownerRole);
            await context.SaveChangesAsync();
        }
    }

    private async Task AsignAllPermissionsTo(Guid RoleId)
    {
        //   in Future you will use it for all permtions 
        var allPermissions = await context.Permissions.ToListAsync();

        allPermissions.ForEach(e=> 
            context.RolePermissions.Add(new RolePermission { 
                RoleId = RoleId, 
                PermissionId = e.Id 
            })
        );

        await context.SaveChangesAsync();
    }

    private async Task SeedClients()
    {
        var firstUser = await context.Users
            .OrderBy(u => u.CreatedAt)
            .FirstOrDefaultAsync();

        if (firstUser is null)
            return;

        var existingCount = await context.Clients
            .CountAsync(c => c.TenantId == firstUser.TenantId);


        var clients = new List<Client>();

        for (int i = existingCount + 1; i <= 20000; i++)
        {
            clients.Add(new Client
            {
                TenantId = firstUser.TenantId,
                AddedById = firstUser.Id,

                CompanyName = $"Company {i}",
                ContactName = $"Contact Person {i}",
                ContactEmail = $"contact{i}@company.com",
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