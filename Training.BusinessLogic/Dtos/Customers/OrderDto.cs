namespace Training.BusinessLogic.Dtos.Customers
{
    public class OrderDto
    {
        public long Id { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<OrderDetailDTO>? OrderDetails { get; set; }
    }
}
