namespace Training.BusinessLogic.Dtos.Auth
{
    public class LoginReqDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public Guid DeviceUuid { get; set; }
    }
}
