using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(i => i.Category)
                .WithMany(i => i.Products)
                .HasForeignKey(i => i.CategoryId);

            builder.HasIndex(i => i.CreatedBy);
            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
