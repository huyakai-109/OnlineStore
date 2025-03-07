namespace Training.BusinessLogic.Dtos.Admin
{
    public class ChangePasswordDto
    {
        public long Id { get; set; }

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }
    }
}
