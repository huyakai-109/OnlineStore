using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    public class Employee : BaseEntity, IIsDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long UserId { get; set; }

        [MaxLength(200)]
        public string FirstName { get; set; }

        [MaxLength(200)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
    }
}
