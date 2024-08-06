using Training.Common.EnumTypes;

namespace Training.Cms.Models
{
    public class CommonSearchViewModel
    {
        public string? SearchQuery { get; set; }
        public int Skip { get; set; } = 1;
        public int Take { get; set; } = 20;
    }

}
