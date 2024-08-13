using Training.Common.EnumTypes;

namespace Training.Cms.Models
{
    public class StockEventViewModel
    {
        public long StockId { get; set; }
        public StockEventType Type { get; set; }
        public string? Reason { get; set; }
        public int Quantity { get; set; }
    }
}
