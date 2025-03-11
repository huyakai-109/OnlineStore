namespace Training.BusinessLogic.Dtos.Customers
{
    public class AddToCartDto
    {
        public long UserId { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
