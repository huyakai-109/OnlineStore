namespace Training.Api.Models.Requests.Carts
{
    public class EditQuantityReq
    {
        public long ProductId { get; set; }
        public int NewQuantity { get; set; }
    }
}
