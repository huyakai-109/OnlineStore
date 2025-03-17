using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Orders")]
    public class Order : BaseEntity, IIsDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long? ClerkId { get; set; }

        public long CustomerId { get; set; }

        [Column(TypeName = "decimal(19,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
