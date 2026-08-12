using gmvTM.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class StopTripItemConfiguration : IEntityTypeConfiguration<StopTripItem>
    {
        public void Configure(EntityTypeBuilder<StopTripItem> builder)
        {
            builder.ToTable(Tables.StopTrips);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.StopCode).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
            builder.HasIndex(x => new { x.TripID, x.Sequence }).IsUnique();
            builder.HasOne<StopItem>().WithMany().HasForeignKey(x => x.StopID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
