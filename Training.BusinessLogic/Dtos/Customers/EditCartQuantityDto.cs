using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class EditCartQuantityDto
    {
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public int  NewQuantity { get; set; }
    }
}
