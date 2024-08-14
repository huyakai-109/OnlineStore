namespace Training.Cms.Models
{
    public class OrderViewModel
    {
        public string? CustomerName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public float TotalPrice { get; set; }
    }
}
