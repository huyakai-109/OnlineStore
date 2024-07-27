namespace Training.Api.Models.Responses.Products
{
    public class ProductRes
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public float UnitPrice { get; set; }
        public string? Category { get; set; }
    }
}
