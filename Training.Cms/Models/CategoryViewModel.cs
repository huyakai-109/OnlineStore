using System.ComponentModel.DataAnnotations;

namespace Training.Cms.Models
{
    public class CategoryViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(255)]
        public string? Image { get; set; }
    }
}
