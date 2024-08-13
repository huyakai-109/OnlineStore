namespace Training.Cms.Models
{
    public class StockViewModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Product { get; set; }
        public string? Category { get; set; }
    }
}
