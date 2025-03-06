using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Users")]
    public class User : IdentityUser<long>, IBaseEntity, IIsDeletedEntity
    {
        public DateTimeOffset LastLogin { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsAdmin { get; set; }

        public long CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
