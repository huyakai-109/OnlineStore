using Training.BusinessLogic.Dtos.Customers;

namespace Training.Api.Models.Responses.Users
{
 
    public class LoginRes
    {
        public string? Token { get; set; }
        public CustomerDto? User { get; set; }
    }
}
