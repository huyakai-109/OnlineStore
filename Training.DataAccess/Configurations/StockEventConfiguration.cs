using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    internal class StockEventConfiguration : IEntityTypeConfiguration<StockEvent>
    {
        public void Configure(EntityTypeBuilder<StockEvent> builder)
        {
            builder.HasOne(i => i.Stock)
                 .WithMany(i => i.StockEvents)
                 .HasForeignKey(i => i.StockId);

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
