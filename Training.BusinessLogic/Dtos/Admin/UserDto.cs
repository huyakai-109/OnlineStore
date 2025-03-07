namespace Training.BusinessLogic.Dtos.Admin
{
    public class UserDto
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsAdmin { get; set; }

    }
}
