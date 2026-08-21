namespace Infrastructure.Data.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class ClientConfiguration : BaseConfig<Client>, IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        ConfigureBase(builder);

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Client)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i=> i.CompanyName).HasMaxLength(300);
        builder.Property(i=> i.ContactEmail).HasMaxLength(300);
        builder.Property(i=> i.ContactName).HasMaxLength(300);
        builder.Property(i=> i.ContactPhone).HasMaxLength(300);
        builder.Property(i=> i.ContactAddress).HasMaxLength(300);
        builder.Property(i=> i.TradeLicenseNumber).HasMaxLength(300);

        builder.HasIndex(i=> i.ContactEmail).IsUnique();
        builder.HasIndex(i=> i.TradeLicenseNumber).IsUnique();
        builder.HasIndex(i=> i.ContactPhone).IsUnique();

    }
}