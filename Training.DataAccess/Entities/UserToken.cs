using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("UserTokens")]
    public class UserToken : IdentityUserToken<long>, IBaseEntity
    {
        public DateTimeOffset RefreshTokenExpiryTime { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public Guid DeviceUuid { get; set; }

        public long CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
