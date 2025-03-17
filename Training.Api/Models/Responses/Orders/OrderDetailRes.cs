namespace Training.Api.Models.Responses.Orders
{
    public class OrderDetailRes
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public float UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string? ProductName { get; set; }

        public string? Thumbnail { get; set; }
    }
}
