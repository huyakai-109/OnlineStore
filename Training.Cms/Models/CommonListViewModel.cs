using Training.BusinessLogic.Common;

namespace Training.Cms.Models
{
    public class CommonListViewModel<T>
    {
        public List<T> Items { get; set; }
        public Pagination Pagination { get; set; }
    }
}
