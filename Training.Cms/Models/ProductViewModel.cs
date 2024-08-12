using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Training.BusinessLogic.Dtos.Admin;

namespace Training.Cms.Models
{
    public class ProductViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? Thumbnail { get; set; } 

        [Required(ErrorMessage = "The Unit Price field is required.")]
        [Range(0, int.MaxValue)]
        public float UnitPrice { get; set; }

     
        public string? Category { get; set; }
        public long CategoryId { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Required(ErrorMessage = "The Stock Quantity field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid stock quantity.")]
        public int StockQuantity { get; set; }

        public bool IsDeleted { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>(); 
    }
}
