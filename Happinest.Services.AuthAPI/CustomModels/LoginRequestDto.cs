using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class LoginRequestDto
    {
        /// <summary>
        /// The username or email of the user trying to log in.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password associated with the user account.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The unique identifier for the device used during login.
        /// This can be used for tracking sessions or managing multi-device logins.
        /// </summary>
        public string? DeviceId { get; set; }
        /// <summary>
        /// The identifier of the guest user.  
        /// If provided, it represents a temporary guest account that can be  
        /// linked or migrated to a registered user account.  
        /// </summary>
        public int? GuestId { get; set; }
    }
}
