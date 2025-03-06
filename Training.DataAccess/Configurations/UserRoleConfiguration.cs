using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Common.Constants;
using Training.DataAccess.Entities;


namespace Training.DataAccess.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(userRole => new { userRole.UserId, userRole.RoleId });

            builder.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId);

            builder.HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId);

            builder.HasData(
                new UserRole
                {
                    UserId = UserConstants.AdminId,
                    RoleId = RolePolicies.Admin.Id,
                    CreatedAt = new DateTime(2025, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(8363),
                    UpdatedAt = new DateTime(2025, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(8363),
                    CreatedBy = 1,
                    UpdatedBy = 1,
                });
        }
    }
}
