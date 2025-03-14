using System.ComponentModel.DataAnnotations.Schema;

namespace Training.DataAccess.Entities
{
    [Table("DeliveryMethods")]
    public class DeliveryMethod : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public required string ShortName { get; set; }

        public required string DeliveryTime { get; set; }

        public required string Description { get; set; }

        [Column(TypeName = "decimal(19,2)")]
        public decimal Price { get; set; }
    }
}
