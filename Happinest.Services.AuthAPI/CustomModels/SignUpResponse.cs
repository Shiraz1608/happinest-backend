namespace Happinest.Services.AuthAPI.CustomModels
{
    public class SignUpResponse : BaseResponse
    {
        public long ServerUserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int? Status { get; set; }
        public string SignUpSource { get; set; }
        public bool ToolTipsVisited { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
