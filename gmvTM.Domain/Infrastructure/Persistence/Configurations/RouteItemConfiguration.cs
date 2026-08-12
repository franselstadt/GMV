using gmvTM.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class RouteItemConfiguration : IEntityTypeConfiguration<RouteItem>
    {
        public void Configure(EntityTypeBuilder<RouteItem> builder)
        {
            builder.ToTable(Tables.Routes);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ShortName).HasMaxLength(32).IsRequired();
            builder.Property(x => x.LongName).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Color).HasMaxLength(32);
            builder.Property(x => x.EncodedPolyline).IsRequired();
        }
    }
}
