namespace Happinest.Services.AuthAPI.Models
{
    public partial class GuestSession
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public string? RefreshToken { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ExpiredOn { get; set; }

        public long? UserId { get; set; }

        public virtual AppUser? User { get; set; }
    }
}
