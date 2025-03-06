using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    public class BaseEntity : IBaseEntity
    {
        public long CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
