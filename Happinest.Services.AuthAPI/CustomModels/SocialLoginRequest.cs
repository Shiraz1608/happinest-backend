namespace Happinest.Services.AuthAPI.CustomModels
{
    public class SocialLoginRequest
    {
        public string AccessToken { get; set; }
        public string DeviceId { get; set; }
        public string AuthenticationSource { get; set; }
        public int? GuestId { get; set; }
    }
}
