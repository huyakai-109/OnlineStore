using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Common.Constants;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            var deliveryMethods = new List<DeliveryMethod>()
            {
                new DeliveryMethod()
                {
                    Id = 1,
                    ShortName = "UPS1",
                    DeliveryTime = "1-2 Days",
                    Description = "Fastest delivery time",
                    Price = 10,
                    CreatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = RolePolicies.Admin.Id,
                    UpdatedBy = RolePolicies.Admin.Id,
                },
                new DeliveryMethod()
                {
                    Id = 2,
                    ShortName = "UPS2",
                    DeliveryTime = "2-5 Days",
                    Description = "Get it within 5 days",
                    Price = 5,
                    CreatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = RolePolicies.Admin.Id,
                    UpdatedBy = RolePolicies.Admin.Id,
                },
                new DeliveryMethod()
                {
                    Id = 3,
                    ShortName = "UPS3",
                    DeliveryTime = "5-10 Days",
                    Description = "Slower but cheap",
                    Price = 2,
                    CreatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = RolePolicies.Admin.Id,
                    UpdatedBy = RolePolicies.Admin.Id,
                },
                new DeliveryMethod()
                {
                    Id = 4,
                    ShortName = "FREE",
                    DeliveryTime = "1-2 Weeks",
                    Description = "Free! You get what you pay for",
                    Price = 0,
                    CreatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = RolePolicies.Admin.Id,
                    UpdatedBy = RolePolicies.Admin.Id,
                }
            };
            
            builder.HasData(deliveryMethods);
        }
    }
}
