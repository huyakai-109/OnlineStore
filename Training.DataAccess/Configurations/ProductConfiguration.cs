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
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Thumbnail).HasMaxLength(255);
            builder.Property(p => p.UnitPrice).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder.HasOne(p => p.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(p => p.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Category)
                   .WithMany()
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
