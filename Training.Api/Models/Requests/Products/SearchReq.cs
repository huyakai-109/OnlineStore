namespace Training.Api.Models.Requests.Products
{
    public class SearchReq
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public int Skip { get; set; } = 0;

        public int Take { get; set; } = 20;
        public bool Ascending { get; set; } = true;
    }
}
