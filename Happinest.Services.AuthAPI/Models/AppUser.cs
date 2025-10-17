using static Happinest.Services.AuthAPI.Helpers.Constant;

namespace Happinest.Services.AuthAPI.Models
{
    public partial class AppUser
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's mobile phone number.
        /// </summary>
        public string? Mobile { get; set; }

        /// <summary>
        /// Gets or sets the user's email address. This field is required and used for authentication.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's hashed password. This field is required for authentication.
        /// </summary>
        public string Password { get; set; } = null!;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Gender { get; set; }

        public int? Age { get; set; }

        public string? Nationality { get; set; }

        public string? Preferences { get; set; }

        public string? AboutUser { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user registered.
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this record was last modified.
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// Gets or sets the user's account status (Active, Inactive, etc.).
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is currently logged in.
        /// </summary>
        public bool? IsloggedIn { get; set; }

        public string? IdentificationMark { get; set; }

        public string? SignUpSource { get; set; }

        public string? DisplayPicture { get; set; }

        public DateTime? Birthday { get; set; }

        public string? BackGroundPicture { get; set; }

        public int? CountryId { get; set; }

        public string? DeviceId { get; set; }

        public string? DisplayName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? AppleUserId { get; set; }

        public DateTime TermsAcceptedDate { get; set; }

        public string? DeviceToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for JWT authentication.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiry time for the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
