using System.ComponentModel.DataAnnotations;

namespace Training.Cms.Models
{
    public class ProductImageViewModel
    {
        public long Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid order.")]
        public int Order { get; set; }

        [StringLength(255)]
        public string? Path { get; set; }
        public IFormFile? Image { get; set; }  // For uploading the image file
        public long ProductId { get; set; }
        public bool IsThumbnail { get; set; }
    }
}
