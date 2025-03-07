using System.ComponentModel.DataAnnotations;

namespace Training.Api.Models.Requests.Users
{
    public class LogoutReq
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        [Required]
        public required Guid DeviceUuid { get; set; }
    }
}
