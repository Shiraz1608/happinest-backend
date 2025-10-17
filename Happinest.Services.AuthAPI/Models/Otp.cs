namespace Happinest.Services.AuthAPI.Models
{
    public partial class Otp
    {
        public long OtpId { get; set; }

        public string Email { get; set; } = null!;

        public int Otpcode { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
