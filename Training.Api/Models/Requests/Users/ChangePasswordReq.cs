namespace Training.Api.Models.Requests.Users
{
    public class ChangePasswordReq
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatNewPassword { get; set; }
    }
}
