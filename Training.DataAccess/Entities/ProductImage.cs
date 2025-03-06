using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Training.DataAccess.Entities
{

    [Table("ProductImages")]
    public class ProductImage : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int Order { get; set; }

        public long ProductId { get; set; }

        public string Path { get; set; }

        public virtual Product Product { get; set; }
    }

}
