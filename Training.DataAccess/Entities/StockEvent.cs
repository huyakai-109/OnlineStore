using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("StockEvents")]
    public class StockEvent: IBaseEntity
    {
        public long Id { get; set; }
        public long StockId { get; set; }
        public int Type { get; set; } // 1 for In, 2 for Out
        public string Reason { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Stock Stock { get; set; }
      
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
