using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasOne(i => i.Order)
                .WithMany(i => i.OrderDetails)
                .HasForeignKey(i => i.OrderId);

            builder.HasOne(i => i.Product)
                .WithMany(i => i.OrderDetails)
                .HasForeignKey(i => i.ProductId);

            builder.HasOne(i => i.Product)
               .WithMany()
               .HasForeignKey(i => i.OrderId);
        }
    }
}
