using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class PurchaseCartDto
    {
        public long UserId { get; set; }
        public long CartId { get; set; }
    }
}
