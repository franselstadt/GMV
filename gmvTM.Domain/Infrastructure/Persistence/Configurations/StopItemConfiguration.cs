using gmvTM.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class StopItemConfiguration : IEntityTypeConfiguration<StopItem>
    {
        public void Configure(EntityTypeBuilder<StopItem> builder)
        {
            builder.ToTable(Tables.Stops);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.StopCode).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
            builder.HasIndex(x => new { x.RouteID, x.StopCode }).IsUnique();
            builder.HasIndex(x => new { x.RouteID, x.Sequence }).IsUnique();
            builder.HasOne<RouteItem>().WithMany().HasForeignKey(x => x.RouteID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
