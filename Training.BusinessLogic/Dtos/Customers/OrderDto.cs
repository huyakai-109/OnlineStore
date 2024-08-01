using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class OrderDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<OrderDetailDTO>? OrderDetails { get; set; }
    }
}
