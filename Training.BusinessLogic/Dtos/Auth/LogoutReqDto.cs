namespace Training.BusinessLogic.Dtos.Auth
{
    public class LogoutReqDto
    {
        public required string Token { get; set; }

        public required string RefreshToken { get; set; }

        public Guid DeviceUuid { get; set; }
    }
}
