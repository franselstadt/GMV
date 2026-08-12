using gmvTM.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gmvTM.Domain.Infrastructure.Persistence.Configurations
{
    public sealed class StopPlanItemConfiguration : IEntityTypeConfiguration<StopPlanItem>
    {
        public void Configure(EntityTypeBuilder<StopPlanItem> builder)
        {
            builder.ToTable(Tables.StopPlans);

            builder.HasKey(x => x.ID);
            builder.HasIndex(x => x.StopID).IsUnique();
            builder.HasOne<StopItem>().WithMany().HasForeignKey(x => x.StopID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
