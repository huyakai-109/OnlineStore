using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.CreatedAt).IsRequired();
            builder.Property(o => o.IsDeleted).HasDefaultValue(false);
            builder.HasOne(o => o.Clerk)
                   .WithMany()
                   .HasForeignKey(o => o.ClerkId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.Customer)
                   .WithMany()
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
