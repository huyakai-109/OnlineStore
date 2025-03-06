using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("RoleClaims")]
    public class RoleClaim : IdentityRoleClaim<long>, IBaseEntity
    {
        public virtual Role Role { get; set; } = null!;

        public long CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
