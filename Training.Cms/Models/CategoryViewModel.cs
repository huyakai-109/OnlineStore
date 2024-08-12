using System.ComponentModel.DataAnnotations;

namespace Training.Cms.Models
{
    public class CategoryViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public IFormFile? Image { get; set; } // support uploading photos

        [StringLength(255)]
        public string? ImagePath { get; set; } // To store the path in the database
    }
}
