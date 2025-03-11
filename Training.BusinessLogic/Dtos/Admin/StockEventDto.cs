using Training.Common.EnumTypes;

namespace Training.BusinessLogic.Dtos.Admin
{
    public class StockEventDto
    {
        public long StockId { get; set; }

        public StockEventType Type { get; set; } 

        public string? Reason { get; set; }

        public int Quantity { get; set; }

        public string? Product { get; set; }

        public string? Category { get; set; }
    }
}
