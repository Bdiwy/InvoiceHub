namespace Infrastructure.Data.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class InvoiceConfiguration : BaseConfig<Invoice>, IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        ConfigureBase(builder);

        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i=> i.InvoiceNumber).HasMaxLength(300);
        builder.Property(i=> i.Title).HasMaxLength(300);
        builder.Property(i=> i.Description).HasMaxLength(600);

        builder.HasIndex(i => new { i.TenantId, i.InvoiceNumber })
            .HasDatabaseName("IX_Invoices_Tenant_InvoiceNumber");
        
        builder.HasIndex(i => new { i.TenantId, i.PaymentMethod })
            .HasDatabaseName("IX_Invoices_Tenant_PaymentMethod");
    }
}