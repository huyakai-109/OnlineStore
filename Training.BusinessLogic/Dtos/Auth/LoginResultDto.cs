namespace Training.BusinessLogic.Dtos.Auth
{
    public class LoginResultDto
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public bool IsAdmin { get; set; }
    }
}
