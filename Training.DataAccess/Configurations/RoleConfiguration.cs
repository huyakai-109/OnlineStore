using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Common.Constants;
using Training.DataAccess.Entities;

namespace Training.DataAccess.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(role => role.Id).HasName("RoleId");
            builder.HasIndex(role => role.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
            builder.Property(role => role.ConcurrencyStamp).IsConcurrencyToken();
            builder.Property(role => role.Name).HasMaxLength(256);
            builder.Property(role => role.NormalizedName).HasMaxLength(256);

            builder.HasData([
                CreateRole(RolePolicies.Admin.Id, RolePolicies.Admin.Name, RolePolicies.Admin.DisplayName),
                CreateRole(RolePolicies.Clerk.Id, RolePolicies.Clerk.Name, RolePolicies.Clerk.DisplayName),
                CreateRole(RolePolicies.Customer.Id, RolePolicies.Customer.Name, RolePolicies.Customer.DisplayName),
            ]);
        }

        private static Role CreateRole(long id, string code, string displayName) => new()
        {
            Id = id,
            Name = code,
            NormalizedName = code,
            DisplayName = displayName,
            ConcurrencyStamp = "616f1653-48e9-4a6f-81b3-1bdd52e565b5",
            CreatedAt = new DateTime(2025, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(8363),
            UpdatedAt = new DateTime(2025, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(8363),
            CreatedBy = 1,
            UpdatedBy = 1,
        };
    }
}
