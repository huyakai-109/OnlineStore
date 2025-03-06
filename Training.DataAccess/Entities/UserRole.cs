using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<long>, IBaseEntity
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }

        public long CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
