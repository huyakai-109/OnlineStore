namespace Training.Api.Models.Responses.Users
{
    public class ProfileRes
    {
        public long Id { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? AvatarUrl {  get; set; }
    }
}
