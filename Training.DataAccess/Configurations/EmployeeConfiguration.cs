using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasOne(i => i.User)
                .WithOne()
                .HasForeignKey<Employee>(i => i.UserId);

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
