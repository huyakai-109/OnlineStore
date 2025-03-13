namespace Training.BusinessLogic.Dtos.Customers
{
    public class CartDto
    {
        public long Id { get; set; }

        public List<CartItemDto> CartItems { get; set; } = [];

        public long? DeliveryMethodId {  get; set; }

        public string? ClientSecret { get; set; }

        public string? PaymentIntentId { get; set; }
    }
}
