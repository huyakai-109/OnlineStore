namespace Training.Api.Models.Responses.Orders
{
    public class OrderRes
    {
        public long Id { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<OrderDetailRes>? OrderDetails { get; set; }
    }
}
