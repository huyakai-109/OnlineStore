using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class OrderDetailDTO
    {
        public int OrderId { get; set; }
        public long ProductId { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        //public string? Thumbnail { get; set; }
    }
}
