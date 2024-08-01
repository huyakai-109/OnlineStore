namespace Training.Api.Models.Requests.Carts
{
    public class AddToCartReq
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
