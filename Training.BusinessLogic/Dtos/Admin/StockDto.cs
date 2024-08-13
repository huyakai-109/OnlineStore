using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class StockDto
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Product { get; set; }
        public string? Category { get; set; }
    }
}
