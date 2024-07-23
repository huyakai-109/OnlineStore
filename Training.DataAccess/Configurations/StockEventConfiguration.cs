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
    internal class StockEventConfiguration : IEntityTypeConfiguration<StockEvent>
    {
        public void Configure(EntityTypeBuilder<StockEvent> builder)
        {
            builder.HasKey(se => se.Id);
            builder.Property(se => se.Id).ValueGeneratedOnAdd();
            builder.Property(se => se.Type).IsRequired();
            builder.Property(se => se.Reason).HasMaxLength(500);
            builder.Property(se => se.Quantity).IsRequired();
            builder.Property(se => se.CreatedAt).IsRequired();
            builder.HasOne(se => se.Stock)
                   .WithMany()
                   .HasForeignKey(se => se.StockId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
