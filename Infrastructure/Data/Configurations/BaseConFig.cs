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

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedBy)
           .WithMany()
           .HasForeignKey(e => e.UpdatedById)
           .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.DeletedBy)
           .WithMany()
           .HasForeignKey(e => e.DeletedById)
           .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new
        {
            e.TenantId,
            e.CreatedById
        });

        builder.HasIndex(e => new
        {
            e.TenantId,
            e.UpdatedById
        });
    }
}