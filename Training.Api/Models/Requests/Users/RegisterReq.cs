using System.ComponentModel.DataAnnotations;

namespace Training.Api.Models.Requests.Users
{
    public class RegisterReq
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string RepeatPassword { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}
