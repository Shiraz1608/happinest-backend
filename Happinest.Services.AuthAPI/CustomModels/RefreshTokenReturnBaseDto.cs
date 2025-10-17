namespace Happinest.Services.AuthAPI.CustomModels
{
    public class RefreshTokenReturnBaseDto : BaseResponse
    {
        public RefreshTokenReturnDto Data { get; set; }
    }

    public class RefreshTokenReturnDto
    {
        public string Token { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string RefreshToken { get; set; }
    }
}
