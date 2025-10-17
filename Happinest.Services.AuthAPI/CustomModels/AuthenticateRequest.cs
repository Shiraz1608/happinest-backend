using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public bool OverWriteExistingSession { get; set; } = false;
        public string DeviceId { get; set; }
        public string? AuthenticationSource { get; set; }
        public string? AppleUserId { get; set; }
    }
}
