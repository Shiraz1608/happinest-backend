using Happinest.Services.AuthAPI.CustomModels;
using Happinest.Services.AuthAPI.CustomModels.SharedModels;
using Happinest.Services.AuthAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Happinest.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        private readonly ITokenService _tokenService;
        public AuthController(IAuthServices authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }
        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<SignUpResponse> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                string[] Name = [""];
                string firstName = "";
                string LastName = "";
                if (signUpRequest.FullName.Contains(" "))
                {
                    Name = signUpRequest.FullName.Split(" ");
                    firstName = Name[0];
                    LastName = Name[1];
                }
                else
                {
                    firstName = signUpRequest.FullName;
                }

                    var auth = await _authService.SignUp(signUpRequest.Email,
                                                        signUpRequest.Password,
                                                        signUpRequest.SignUpSource,
                                                        signUpRequest.DeviceId,
                                                        signUpRequest.AppleUserId,
                                                        signUpRequest.GeneratedOtp,
                                                        signUpRequest.GuestId,
                                                        firstName,
                                                        LastName,
                                                        signUpRequest.FullName);

                return auth;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest authenticateRequest)
        {
            try
            {
                var auth = await _authService.Authenticate(authenticateRequest.Email, authenticateRequest.Password, authenticateRequest.OverWriteExistingSession, authenticateRequest.DeviceId, authenticateRequest.AuthenticationSource, authenticateRequest.AppleUserId);

                return auth;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<AuthenticateResponse> Login(LoginRequestDto request)
        {
            try
            {
                return await _authService.Login(request);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpPost("GenerateOtp")]
        public async Task<BaseResponse> GenerateOtp([FromBody] GenerateOtpRequest generateOtpRequest)
        {
            try
            {
                BaseResponse response = await _authService.GenerateOtp(generateOtpRequest.Email);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(CustomModels.ForgotPasswordRequest resetPasswordRequest)
        {
            try
            {
                var response = await _authService.ForgotPassword(resetPasswordRequest);

                if (response.Success)
                {
                    return Ok(response); // 200 OK
                }
                else
                {
                    return BadRequest(response); // 400 Bad Request
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(CustomModels.SharedModels.ResetPasswordRequest request)
        {
            try
            {
                var response = await _authService.ResetPassword(request);

                // Wrap your ApiResponse in an IActionResult
                if (response.Success)
                {
                    return Ok(response); // 200 OK
                }
                else
                {
                    return BadRequest(response); // 400 Bad Request
                }
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return StatusCode(500, "An unexpected error occurred");
            }
        }
        [AllowAnonymous]
        [HttpPost("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP(CustomModels.SharedModels.ValidateOTP request)
        {
            try
            {
                var response = await _authService.ValidateOTP(request);

                // Wrap your ApiResponse in an IActionResult
                    if (response.Success)
                {
                    return Ok(response); // 200 OK
                }
                else
                {
                    return BadRequest(response); // 400 Bad Request
                }
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<BaseResponse> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                return await _authService.ChangePassword(changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<RefreshTokenReturnBaseDto> RefreshToken(CustomModels.RefreshRequest request)
        {
            try
            {
                return await _authService.RefreshToken(request);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("guestLogin")]
        [AllowAnonymous]
        public async Task<AuthenticateResponse> GuestLogin()
        {
            try
            {
                return await _authService.GuestLogin();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("SocialLogin")]
        [AllowAnonymous]
        public Task<AuthenticateResponse> SocialLogin(SocialLoginRequest request)
        {
            try
            {
                return _authService.SocialLoginAsync(request);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("Logout")]
        public async Task<LogoutResonse> Logout()
        {
            try
            {
                return await _authService.Logout();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost("UpdateDeviceToken")]
        public async Task<BaseResponse> UpdateDeviceToken(DeviceTokenRequest tokenRequest)
        {
            try
            {
                return await _authService.UpdateDeviceToken(tokenRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
