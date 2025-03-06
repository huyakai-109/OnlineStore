using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{

    [Table("OrderDetails")]
    public class OrderDetail : BaseEntity, IIsDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public float UnitPrice { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
