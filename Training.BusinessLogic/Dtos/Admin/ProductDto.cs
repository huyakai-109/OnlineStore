using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public float UnitPrice { get; set; }
        public string? CreatedBy { get; set; } 
        public string? Category { get; set; }
        public long CategoryId { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDeleted { get; set; }
    }

}
