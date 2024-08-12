using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class ProductImageDto
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public string? Path { get; set; }
        public long ProductId { get; set; }
        public bool IsThumbnail { get; set; }
    }
}
