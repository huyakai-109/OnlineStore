using System.ComponentModel.DataAnnotations;
using Training.Common.EnumTypes;

namespace Training.Cms.Models
{
    public class UserViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [StringLength(12)]
        public string? CivilianId { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(11)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
