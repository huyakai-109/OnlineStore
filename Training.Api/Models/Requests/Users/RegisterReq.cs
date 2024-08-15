namespace Training.Api.Models.Requests.Users
{
    public class RegisterReq
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RepeatPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CivilianId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
