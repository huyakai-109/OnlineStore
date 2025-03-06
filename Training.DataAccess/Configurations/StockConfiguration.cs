using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    internal class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.HasOne(i => i.Product)
                   .WithOne()
                   .HasForeignKey<Stock>(i => i.ProductId);

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
