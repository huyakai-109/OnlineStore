using System.ComponentModel.DataAnnotations;

namespace Training.Cms.Models
{
    public class LoginVM
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        public required Guid DeviceUuid { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
}
