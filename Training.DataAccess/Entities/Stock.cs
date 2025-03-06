using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Stocks")]
    public class Stock : BaseEntity, IIsDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<StockEvent> StockEvents { get; set; }
    }
}
