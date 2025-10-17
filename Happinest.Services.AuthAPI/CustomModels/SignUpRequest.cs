using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Password { get; set; } = null!;
        [Required]
        public string SignUpSource { get; set; }
        public string DeviceId { get; set; }
        public string AppleUserId { get; set; }
        public int? GeneratedOtp { get; set; }

        public int? GuestId { get; set; }
    }
}
