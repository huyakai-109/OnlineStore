using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Stocks")]
    public class Stock:IBaseEntity
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }

        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
