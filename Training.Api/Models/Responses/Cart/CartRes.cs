using Training.BusinessLogic.Dtos.Customers;

namespace Training.Api.Models.Responses.Cart
{
    public class CartRes
    {
        public List<CartItemRes>? CartItems { get; set; }
    }
}
