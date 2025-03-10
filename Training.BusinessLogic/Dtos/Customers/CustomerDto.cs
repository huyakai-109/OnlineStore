using Microsoft.AspNetCore.Http;

namespace Training.BusinessLogic.Dtos.Customers
{
    public class CustomerDto
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}
