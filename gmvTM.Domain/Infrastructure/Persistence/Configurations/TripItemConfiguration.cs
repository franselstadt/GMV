using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class TripItemConfiguration : IEntityTypeConfiguration<TripItem>
    {
        public void Configure(EntityTypeBuilder<TripItem> builder)
        {
            builder.ToTable(Tables.Trips);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.Status).HasMaxLength(64).IsRequired();

            builder.HasOne<RouteItem>().WithMany().HasForeignKey(x => x.RouteID).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<StopItem>().WithMany().HasForeignKey(x => x.StartStopID).OnDelete(DeleteBehavior.Restrict);
           
            builder.HasMany(x => x.StopTrips)
                .WithOne()
                .HasForeignKey(x => x.TripID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.StopTrips).AutoInclude();
        }
    }
}
