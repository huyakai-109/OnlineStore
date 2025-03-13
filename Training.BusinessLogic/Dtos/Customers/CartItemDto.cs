namespace Training.BusinessLogic.Dtos.Customers
{
    public class CartItemDto
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Thumbnail { get; set; }

        public float Price { get; set; }

        public int Quantity { get; set; }
    }
}
