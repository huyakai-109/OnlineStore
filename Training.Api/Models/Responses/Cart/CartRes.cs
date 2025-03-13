namespace Training.Api.Models.Responses.Cart
{
    public class CartRes
    {
        public long Id { get; set; }

        public List<CartItemRes>? CartItems { get; set; }

        public long? DeliveryMethodId { get; set; }

        public string? ClientSecret { get; set; }

        public string? PaymentIntentId { get; set; }
    }
}
