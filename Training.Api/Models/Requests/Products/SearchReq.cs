namespace Training.Api.Models.Requests.Products
{
    public class SearchReq
    {
        public string? SearchQuery { get; set; }
        public int Skip { get; set; } = 1;
        public int Take { get; set; } = 20;
        public bool Ascending { get; set; } = true;
    }
}
