using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.BusinessLogic.Dtos.Base
{
    public class CommonSearchDto : SearchDto
    {
        public string? SearchQuery { get; set; }

    }
}
