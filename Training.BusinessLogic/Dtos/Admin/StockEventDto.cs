using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Common.EnumTypes;
using Training.DataAccess.Entities;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class StockEventDto
    {
        public long StockId { get; set; }
        public StockEventType Type { get; set; } 
        public string? Reason { get; set; }
        public int Quantity { get; set; }

    }
}
