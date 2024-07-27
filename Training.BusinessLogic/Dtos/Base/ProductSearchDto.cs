using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Base
{
    public class ProductSearchDto : SearchDto
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public bool SortByPriceAscending { get; set; } = true;
    }
}
