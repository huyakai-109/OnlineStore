using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Carts")]  
    public class Cart: IBaseEntity, IIsDeletedEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public bool IsPurchased { get; set; }
        public virtual User User { get; set; }
        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
