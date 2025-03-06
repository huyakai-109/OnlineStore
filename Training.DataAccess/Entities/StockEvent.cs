using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Training.Common.EnumTypes;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("StockEvents")]
    public class StockEvent: BaseEntity, IIsDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long StockId { get; set; }

        public StockEventType Type { get; set; } // 1 for In, 2 for Out

        [MaxLength(500)]
        public string Reason { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Stock Stock { get; set; }
    }
}
