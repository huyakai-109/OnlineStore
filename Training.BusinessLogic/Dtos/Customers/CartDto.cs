using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.Entities;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class CartDto
    {
        public List<CartItemDto>? CartItems { get; set; }
    }
}
