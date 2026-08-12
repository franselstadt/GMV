using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class VehicleItemConfiguration : IEntityTypeConfiguration<VehicleItem>
    {
        public void Configure(EntityTypeBuilder<VehicleItem> builder)
        {
            builder.ToTable(Tables.Vehicles);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.FleetCode).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Make).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Model).HasMaxLength(64).IsRequired();
            builder.Property(x => x.LicensePlate).HasMaxLength(32).IsRequired();
            builder.HasIndex(x => x.FleetCode).IsUnique();
            builder.HasIndex(x => x.LicensePlate).IsUnique();

            builder.HasMany(x => x.Trips)
                .WithOne()
                .HasForeignKey(x => x.VehicleID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
