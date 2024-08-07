using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}
