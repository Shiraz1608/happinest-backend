using Happinest.Services.AuthAPI.CustomModels;
using Happinest.Services.AuthAPI.CustomModels.SharedModels;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Happinest.Services.AuthAPI.Interfaces
{
    public interface IAuthServices
    {
        Task<ApiResponse<string>> ResetPassword(CustomModels.SharedModels.ResetPasswordRequest request);
        Task<ApiResponse<string>> ValidateOTP(CustomModels.SharedModels.ValidateOTP request);
        Task<SignUpResponse> SignUp(string email, string password, string source, string DeviceId, string appleUserId, int? generatedOtp, int? guestId,string firstName, string lastName, string fullName);
        Task<AuthenticateResponse> Authenticate(string email, string password, bool overWriteExistingSession, string deviceId, string AuthenticationSource, string appleUserId);
        Task<AuthenticateResponse> Login(LoginRequestDto request);
        Task<BaseResponse> GenerateOtp(string email);
        Task<ApiResponse<string>> ForgotPassword(CustomModels.ForgotPasswordRequest request);
        Task<BaseResponse> ChangePassword(string CurrentPassword, string NewPassword);
        Task<RefreshTokenReturnBaseDto> RefreshToken(CustomModels.RefreshRequest refresh);
        Task<AuthenticateResponse> GuestLogin();
        Task<AuthenticateResponse> SocialLoginAsync(SocialLoginRequest request);
        Task<LogoutResonse> Logout();
        Task<BaseResponse> UpdateDeviceToken(DeviceTokenRequest tokenRequest);
    }
}
