using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasOne(i => i.User)
                .WithOne()
                .HasForeignKey<Customer>(i => i.UserId);

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
