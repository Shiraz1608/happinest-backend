using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels.SharedModels
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public int Otp { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class ValidateOTP
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public int Otp { get; set; }
    }
}
