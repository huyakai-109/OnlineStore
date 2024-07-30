using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Orders")]
    public class Order : IBaseEntity, IIsDeletedEntity
    {
        public long Id { get; set; }
        public long ClerkId { get; set; }
        public long CustomerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public User Clerk { get; set; }
        public User Customer { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }

        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
