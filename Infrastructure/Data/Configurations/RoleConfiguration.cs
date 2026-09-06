namespace Infrastructure.Data.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class RoleConfiguration : BaseConfig<Role>, IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ConfigureBase(builder);

        builder.Property(t => t.Name)
                 .IsRequired()
                 .HasMaxLength(300);

        builder.HasMany(t => t.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.TenantId, i.Name })
                .IsUnique()
                .HasDatabaseName("IX_Role_Tenant_Name");

    }
}