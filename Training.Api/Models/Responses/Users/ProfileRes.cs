namespace Training.Api.Models.Responses.Users
{
    public class ProfileRes
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CivilianId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
