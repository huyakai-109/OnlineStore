using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Customers")]
    public class Customer : BaseEntity, IIsDeletedEntity    
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Avatar { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
    }
}
