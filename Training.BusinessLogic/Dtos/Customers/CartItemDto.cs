using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class CartItemDto
    {
        public string? ProductName { get; set; }
        public string? Thumbnail { get; set; }
        public int Quantity { get; set; }
    }
}
