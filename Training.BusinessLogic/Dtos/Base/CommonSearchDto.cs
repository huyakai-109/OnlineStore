using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Common.Constants;

namespace Training.BusinessLogic.Dtos.Base
{
    public class CommonSearchDto : SearchDto
    {
        public string? SearchQuery { get; set; }
        public string Sort { get; set; } = GlobalConstants.SortDirection.Ascending;
    }
}
