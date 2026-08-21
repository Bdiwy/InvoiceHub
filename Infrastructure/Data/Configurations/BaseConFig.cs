namespace Infrastructure.Data.Configurations;

using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public abstract class BaseConfig<TEntity>
    where TEntity : class, IAuditableEntity, ITenantEntity
{
    protected void ConfigureBase(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.AddedBy)
            .WithMany()
            .HasForeignKey(e => e.AddedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new
        {
            e.TenantId,
            e.AddedById
        });
    }
}