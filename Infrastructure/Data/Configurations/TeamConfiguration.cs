using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Data.Configurations
{
    public class TeamConfiguration : BaseConfig<Team>,IEntityTypeConfiguration<Team> 
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            ConfigureBase(builder);

            builder.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(300);

            builder.HasMany(t => t.Users)
                    .WithOne(u => u.Team)
                    .HasForeignKey(u => u.TeamId)
                    .OnDelete(DeleteBehavior.SetNull); 
            
            builder.HasIndex(i => new { i.TenantId, i.Name })
                    .IsUnique()
                    .HasDatabaseName("IX_Team_Tenant_Name");
            }
    }
}