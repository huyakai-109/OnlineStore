using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class OrderDto
    {
        public long Id { get; set; }
        public string? CustomerName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public float TotalPrice { get; set; }   
    }
}
