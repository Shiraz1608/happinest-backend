using Google.Apis.Auth;
using Happinest.Models;
using Happinest.Services.AuthAPI.CustomModels;
using Happinest.Services.AuthAPI.CustomModels.SharedModels;
using Happinest.Services.AuthAPI.DataContext;
using Happinest.Services.AuthAPI.Helpers;
using Happinest.Services.AuthAPI.Interfaces;
using Happinest.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Claims;
using System.Text;
using static Happinest.Services.AuthAPI.Helpers.Constant;
using static System.Net.WebRequestMethods;

namespace Happinest.Services.AuthAPI.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context; 
        private readonly ITokenService _tokenService;
        private readonly string imageUrl = string.Empty;
        private readonly IHttpService _httpService;
        private readonly IEmailService _emailService;


        public AuthServices(IConfiguration configuration, AppDbContext context, ITokenService tokenService, IHttpService httpService, IEmailService emailService)
        {
            _configuration = configuration;
            _context = context;
            _tokenService = tokenService;
            _httpService = httpService;
            _emailService = emailService;

        }

        public async Task<SignUpResponse> SignUp(string email, string password, string source, string deviceId, string appleUserId, int? generatedOtp, int? guestId,string firstName, string lastName, string fullName)
        {
            SignUpResponse signUpResponse = new SignUpResponse { ResponseStatus = false };
            try
            {
                if (source is string str && str.Trim().ToLower() == "string")
                {
                    signUpResponse.ValidationMessage = "Please enter valid source.";
                    return signUpResponse;
                }
                // Check for existing user
                var existingUserDetail = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Status == (int)UserStatus.Active && u.Email == email.Trim() || (source.ToLowerInvariant() == "apple" && u.AppleUserId == appleUserId));

                if (existingUserDetail != null)
                {
                    signUpResponse.ValidationMessage = "Seems like you are already signed up on Happinest. Please log in using existing credentials.";
                    return signUpResponse;
                }

                // Handle inapp source with OTP validation
                if (source.ToLowerInvariant() == "inapp")
                {
                    if (generatedOtp == null)
                    {
                        signUpResponse.ValidationMessage = "Please send generatedOtp if source is InApp.";
                        return signUpResponse;
                    }

                    bool isValid = await ValidateOtp(email, generatedOtp);
                    if (!isValid)
                    {
                        signUpResponse.ValidationMessage = "Otp Expired";
                        return signUpResponse;
                    }
                }

                // Add new user for all valid cases
                signUpResponse = await AddNewUser(email, password, source, deviceId, appleUserId,firstName,lastName,fullName);

                if (string.IsNullOrEmpty(signUpResponse.ValidationMessage))
                {
                    await MapGuestToUserAsync(guestId, signUpResponse.ServerUserId);
                    signUpResponse.Token = await GetTokenAndClaims(email, signUpResponse.ServerUserId);

                    var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshTokenWithExpiry();

                    if (!string.IsNullOrWhiteSpace(refreshToken))
                    {
                        signUpResponse.RefreshToken = refreshToken;
                        signUpResponse.RefreshTokenExpiryTime = refreshTokenExpiry;
                    }

                    signUpResponse.ResponseStatus = true;
                    signUpResponse.ValidationMessage = "Success";
                    signUpResponse.StatusCode = StatusCode.Success;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return signUpResponse;
        }
        private async Task<SignUpResponse> AddNewUser(string email, string password, string source, string deviceId, string appleUserId,string firstName, string lastName, string fullName)
        {

            if (!string.IsNullOrWhiteSpace(email))
            {
                bool emailAlreadyExist = await _context.AppUsers.AnyAsync(x => x.Email == email);

                if (emailAlreadyExist)
                {
                    return new SignUpResponse()
                    {
                        ResponseStatus = false,
                        StatusCode = StatusCode.UserNameExist,
                        ValidationMessage = "Email already exist"
                    };
                }
            }
            SignUpResponse signUpResponse = new SignUpResponse();
            if (string.IsNullOrEmpty(password) || password == "NULL")
            {
                string str = RandomString(4, false);
                password = str;
            }
            AppUser appUser = new AppUser();
            appUser.Email = email;
            appUser.Password = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
            appUser.Status = (int)UserStatus.Active;
            appUser.RegistrationDate = DateTime.UtcNow;
            appUser.CreatedDate = DateTime.UtcNow;
            appUser.SignUpSource = source;
            appUser.IsloggedIn = true;
            appUser.DeviceId = deviceId;
            appUser.DisplayName = fullName;
            appUser.FirstName = firstName;
            appUser.LastName = lastName;
            //appUser.ToolTipsVisited = false;
            if (!string.IsNullOrEmpty(appleUserId))
            {
                appUser.AppleUserId = appleUserId;
            }
            string[] emailParts = email.Split('@');
            //appUser.FirstName = emailParts[0];  
            appUser.DisplayName = emailParts[0] ?? string.Empty;
            appUser.TermsAcceptedDate = DateTime.UtcNow;
            await _context.AppUsers.AddAsync(appUser);
            await _context.SaveChangesAsync();

            #region Add a default user role to the registerd user
            var userRole = await _context.UserRoleMasters
                   .FirstOrDefaultAsync(r => r.RoleName.ToLower() == UserRoles.User.ToString().ToLower()); // or "Admin" if needed

            if (userRole == null)
            {
                throw new Exception("Role 'User' not found in Role table.");
            }

            // 3. Assign role to user
            var appUserRole = new UserRole
            {
                UserId = appUser.UserId,
                RoleId = userRole.Id
            };

            await _context.UserRoles.AddAsync(appUserRole);
            await _context.SaveChangesAsync();
            #endregion

            //return the user details in pagedEvents
            signUpResponse.ServerUserId = appUser.UserId;
            signUpResponse.Email = appUser.Email;
            signUpResponse.FirstName = appUser.FirstName;
            signUpResponse.DisplayName = appUser.FirstName;
            signUpResponse.RegistrationDate = Convert.ToDateTime(appUser.RegistrationDate);
            signUpResponse.Status = appUser.Status;
            signUpResponse.SignUpSource = appUser.SignUpSource;
            signUpResponse.ResponseStatus = true;
            signUpResponse.ToolTipsVisited = false;

            var existingInviteByUserId = await _context.EventGuests
                         .Where(wi => wi.Email == signUpResponse.Email).ToListAsync();

            foreach (var invite in existingInviteByUserId ?? new List<EventGuest>())
            {
                invite.InviteTo = signUpResponse.ServerUserId;

                _context.EventGuests.Update(invite);
                await _context.SaveChangesAsync();
            }

            return signUpResponse;
        }
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                    return builder.ToString().ToLower();

            }
            catch
            {
                throw;
            }
            return builder.ToString();
        }
        private async Task<bool> ValidateOtp(string email, int? generatedOtp)
        {
            Otp otpDetails = await _context.Otps.Where(o => o.Email == email && o.Otpcode == generatedOtp).FirstOrDefaultAsync();
            if (otpDetails != null)
            {
                if (DateTime.UtcNow.Date == otpDetails.CreatedOn.Date)
                {
                    int minute = Convert.ToInt32(DateTime.UtcNow.Subtract(otpDetails.CreatedOn).TotalMinutes);
                    if (minute <= Convert.ToInt16(_configuration["OTPExpiredTime"]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private async Task MapGuestToUserAsync(int? guestId, long userId)
        {
            if (!guestId.HasValue || guestId <= 0 || userId <= 0)
                return;

            try
            {
                var guestSession = await _context.GuestSessions
                    .FirstOrDefaultAsync(g => g.Id == guestId.Value);

                if (guestSession == null)
                    return;

                guestSession.UserId = userId;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Safely pass on any exception 
            }
        }
        private async Task<string> GetTokenAndClaims(string email, long userId)
        {
            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("UserId", userId.ToString()),
                        new Claim(ClaimTypes.Email,  email) // fallback if not set
                    };

            var roles = await _context.UserRoles
                        .Include(x => x.Role)
                        .Where(x => x.UserId == userId)
                        .ToListAsync();

            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, UserRoles.User.ToString()));
            }

            return await _tokenService.GetToken(claims);
        }
        public async Task<AuthenticateResponse> Authenticate(string email, string password, bool overWriteExistingSession, string deviceId, string authenticationSource, string appleUserId)
        {
            AuthenticateResponse authenticateResponse = new AuthenticateResponse();
            try
            {

                authenticateResponse.ResponseStatus = false;

                if (authenticationSource == "apple")
                {
                    authenticateResponse = await GetLoggedInUserDetailForApple(email, authenticationSource, password, deviceId, overWriteExistingSession, appleUserId);
                }
                else
                {
                    authenticateResponse = await GetLoggedInUserDetail(email, authenticationSource, password, deviceId, overWriteExistingSession);
                }
                if (authenticateResponse != null && string.IsNullOrEmpty(authenticateResponse.ValidationMessage))
                {
                    authenticateResponse.Token = await GetTokenAndClaims(email, authenticateResponse.UserId);
                }

                return authenticateResponse;
            }
            catch
            {
                throw;
            }
        }
        private async Task<AuthenticateResponse> GetLoggedInUserDetailForApple(string email, string authenticationSource, string password, string deviceId, bool overWriteExistingSession, string appleUserId)
        {
            AuthenticateResponse authenticateResponse = new AuthenticateResponse();


            var userDetail = await (from obj in _context.AppUsers
                                    where (obj.Email == email || obj.AppleUserId == appleUserId) && obj.Status == (int)UserStatus.Active
                                    select obj).FirstOrDefaultAsync();
            if (userDetail != null)
            {
                // Modified the code to match both email and password
                if (authenticationSource != "InApp" || BCrypt.Net.BCrypt.Verify(password, userDetail.Password))
                {
                    if ((overWriteExistingSession == false && (userDetail.DeviceId == deviceId || userDetail.DeviceId == null || deviceId == "UserPanel")) || (overWriteExistingSession == true))
                    {
                        authenticateResponse.ServerUserId = userDetail.UserId;
                        authenticateResponse.FirstName = userDetail.FirstName;
                        authenticateResponse.LastName = userDetail.LastName;
                        authenticateResponse.Mobile = userDetail.Mobile;
                        authenticateResponse.Email = userDetail.Email;
                        authenticateResponse.Address = userDetail.Address;
                        authenticateResponse.City = userDetail.City;
                        authenticateResponse.State = userDetail.State;
                        authenticateResponse.Gender = userDetail.Gender;
                        authenticateResponse.CountryId = userDetail.CountryId;
                        authenticateResponse.Preferences = userDetail.Preferences;
                        authenticateResponse.AboutUser = userDetail.AboutUser;
                        authenticateResponse.Status = userDetail.Status;
                        authenticateResponse.IsloggedIn = true;
                        authenticateResponse.IdentificationMark = userDetail.IdentificationMark;
                        authenticateResponse.SignUpSource = userDetail.SignUpSource;
                        authenticateResponse.RegistrationDate = userDetail.RegistrationDate;
                        authenticateResponse.UserId = userDetail.UserId;

                        //var settings = await _happinestApiContext.Settings.Select(s => new { s.MinDistance, s.MinDurationInMins }).FirstOrDefaultAsync();
                        //authenticateResponse.MinDistanceForLocationTracking = settings.MinDistance.Value;
                        //authenticateResponse.MinDurationForLocationTracking = settings.MinDurationInMins.Value;

                        //authenticateResponse.ToolTipsVisited = userDetail.ToolTipsVisited;

                        //var RunningTripDetail = (from obj in _happinestApiContext.Trips where obj.Userid == userDetail.UserId && obj.EndTime == null select obj).FirstOrDefault();
                        //if (RunningTripDetail != null)
                        //{
                        //    authenticateResponse.RunningTripId = RunningTripDetail.TripId;
                        //    authenticateResponse.RunningTripName = RunningTripDetail.TripName;
                        //    authenticateResponse.TravelTypeId = RunningTripDetail.TravelTypeId;
                        //}
                        var imageUrl = _configuration["ImagesBaseUrl"];
                        if (!string.IsNullOrEmpty(userDetail.DisplayPicture))
                        {
                            authenticateResponse.UserProfilePictureUrl = imageUrl + "/ProfilePicture/" + userDetail.DisplayPicture;
                        }
                        else
                        {
                            authenticateResponse.UserProfilePictureUrl = imageUrl + "/ProfilePicture/default.png";
                        }
                        userDetail.IsloggedIn = true;
                        if (deviceId != "UserPanel")
                        {
                            userDetail.DeviceId = deviceId;
                        }

                        await _context.SaveChangesAsync();

                        authenticateResponse.ResponseStatus = true;
                    }
                    else
                    {
                        authenticateResponse.ValidationMessage = "You are already logged-in from another device. Do you want to logout from another device?";
                    }
                }
                else
                {
                    authenticateResponse.ValidationMessage = "Invalid username or password.";
                }
            }
            else
            {
                authenticateResponse.ValidationMessage = "You are not yet signed up. Please sign up to start enjoying travelory.";
            }
            //end using
            return authenticateResponse;
        }
        private async Task<AuthenticateResponse> GetLoggedInUserDetail(string email, string authenticationSource, string password, string deviceId, bool overWriteExistingSession)
        {
            try
            {
                AuthenticateResponse authenticateResponse = new AuthenticateResponse();
                var userDetail = await (from obj in _context.AppUsers
                                        .Include(x => x.UserRoles)
                                        where obj.Email == email && obj.Status == (int)UserStatus.Active
                                        select obj).FirstOrDefaultAsync();
                if (userDetail != null)
                {
                    // Modified the code to match both email and password
                    if (authenticationSource != "InApp" || BCrypt.Net.BCrypt.Verify(password, userDetail.Password))
                    {
                        if ((overWriteExistingSession == false && (userDetail.DeviceId == deviceId || userDetail.DeviceId == null || deviceId == "UserPanel")) || (overWriteExistingSession == true))
                        {
                            authenticateResponse.ServerUserId = userDetail.UserId;
                            authenticateResponse.FirstName = userDetail.FirstName;
                            authenticateResponse.LastName = userDetail.LastName;
                            authenticateResponse.Mobile = userDetail.Mobile;
                            authenticateResponse.Email = userDetail.Email;
                            authenticateResponse.Address = userDetail.Address;
                            authenticateResponse.City = userDetail.City;
                            authenticateResponse.State = userDetail.State;
                            authenticateResponse.Gender = userDetail.Gender;
                            authenticateResponse.CountryId = userDetail.CountryId;
                            authenticateResponse.Preferences = userDetail.Preferences;
                            authenticateResponse.AboutUser = userDetail.AboutUser;
                            authenticateResponse.Status = userDetail.Status;
                            authenticateResponse.IsloggedIn = true;
                            authenticateResponse.IdentificationMark = userDetail.IdentificationMark;
                            authenticateResponse.SignUpSource = userDetail.SignUpSource;
                            authenticateResponse.RegistrationDate = userDetail.RegistrationDate;
                            authenticateResponse.UserId = userDetail.UserId;

                            if (authenticateResponse.Roles == null)
                                authenticateResponse.Roles = new List<int>();

                            authenticateResponse.Roles.AddRange(userDetail.UserRoles.Select(x => x.RoleId));

                            var imageUrl = _configuration["ImagesBaseUrl"];
                            if (!string.IsNullOrEmpty(userDetail.DisplayPicture))
                            {
                                authenticateResponse.UserProfilePictureUrl = imageUrl + "/ProfilePicture/" + userDetail.DisplayPicture;
                            }
                            else
                            {
                                authenticateResponse.UserProfilePictureUrl = imageUrl + "/ProfilePicture/default.png";
                            }

                            userDetail.IsloggedIn = true;
                            if (deviceId != "UserPanel")
                            {
                                userDetail.DeviceId = deviceId;
                            }
                            await _context.SaveChangesAsync();

                            authenticateResponse.ResponseStatus = true;
                        }
                        else
                        {
                            authenticateResponse.ValidationMessage = "You are already logged-in from another device. Do you want to logout from another device?";
                        }
                    }
                    else
                    {
                        authenticateResponse.ValidationMessage = "Invalid username or password.";
                    }
                }
                else
                {
                    authenticateResponse.ValidationMessage = "You are not yet signed up. Please sign up to start enjoying travelory.";
                }

                //end using
                return authenticateResponse;
            }
            catch (Exception ex)
            {
                var e = ex;
                throw;
            }

        }


        public async Task<BaseResponse> ChangePassword(string CurrentPassword, string NewPassword)
        {
            string validationStatus = string.Empty;
            long userId = _tokenService.GetUserId();
            try
            {
                BaseResponse sResponse = new BaseResponse();
                var userDetail = await (from obj in _context.AppUsers
                                        where obj.UserId == userId && obj.Status == (int)UserStatus.Active
                                        select obj).FirstOrDefaultAsync();
                if (userDetail != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(CurrentPassword, userDetail.Password))
                    {
                        userDetail.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword, BCrypt.Net.BCrypt.GenerateSalt());
                        _context.SaveChanges();
                        //SendPasswor_happinestApiContexthangeEmail(NewPassword, userDetail.Email);
                    }
                    else
                    {
                        validationStatus = "Current password is incorrect.";
                    }
                }
                else
                {
                    validationStatus = "No active user exist with given details.";
                }
                if (string.IsNullOrEmpty(validationStatus))
                {
                    sResponse.ResponseStatus = true;
                    sResponse.ValidationMessage = "Password changed sucessfully.";
                }
                else
                {
                    sResponse.ResponseStatus = false;
                    sResponse.ValidationMessage = validationStatus;
                }
                return sResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ApiResponse<string>> ResetPassword(CustomModels.SharedModels.ResetPasswordRequest request)
        {
            try
            {
                // Step 1: Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    throw new HttpRequestException("Email, New Password are required.", null, HttpStatusCode.BadRequest);
                }

                // Step 2: Fetch user
                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Status == (int)UserStatus.Active);

                if (user == null)
                {
                    throw new HttpRequestException("Invalid email address.", null, HttpStatusCode.NotFound);
                }

                // Step 3: Fetch OTP record
                var otpEntity = await _context.Otps.FirstOrDefaultAsync(o => o.Email == user.Email);

                if (otpEntity == null)
                {
                    throw new HttpRequestException("No OTP found for this email. Please request a new one.", null, HttpStatusCode.BadRequest);
                }

                //// Step 4: Validate OTP
                //if (!await ValidateOtp(user.Email, request.Otp))
                //{
                //    throw new HttpRequestException("Invalid or expired OTP.", null, HttpStatusCode.BadRequest);
                //}

                // Step 6: Update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, BCrypt.Net.BCrypt.GenerateSalt());

                // Step 7: Remove OTP record after successful verification
                _context.Otps.Remove(otpEntity);

                await _context.SaveChangesAsync();

                // Step 8: Return success
                return ApiResponse<string>.SuccessResponse("Password has been reset successfully.");
            }
            catch (HttpRequestException)
            {
                throw; // Allow middleware/global handler to return a formatted HTTP response
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.InnerException?.Message ?? ex.Message ?? "An unexpected error occurred while resetting the password.",
                                                ex, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> ForgotPassword(CustomModels.ForgotPasswordRequest request)
        {
            try
            {
                // Step 1: Validate input
                if (string.IsNullOrWhiteSpace(request.EmailAddress))
                {
                    throw new HttpRequestException("Email address is required.", null, HttpStatusCode.BadRequest);
                }

                // Step 2: Fetch user
                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == request.EmailAddress && u.Status == (int)UserStatus.Active);

                if (user == null)
                {
                    throw new HttpRequestException("Invalid email address.", null, HttpStatusCode.NotFound);
                }

                // Step 3: Generate OTP
                var random = new Random();
                int generatedOtp = random.Next(100000, 999999);

                // Step 4: Check existing OTP record
                var existingOtp = await _context.Otps
                    .FirstOrDefaultAsync(o => o.Email == user.Email);

                if (existingOtp != null)
                {
                    existingOtp.Otpcode = generatedOtp;
                    existingOtp.CreatedOn = DateTime.UtcNow;
                    _context.Otps.Update(existingOtp);
                }
                else
                {
                    var otp = new Otp
                    {
                        Email = user.Email,
                        Otpcode = generatedOtp,
                        CreatedOn = DateTime.UtcNow
                    };
                    await _context.Otps.AddAsync(otp);
                }

                // Step 5: Send OTP via email
                bool isSuccess = await SendOtpByEmailAsync(generatedOtp, user.Email);

                if (!isSuccess)
                {
                    throw new HttpRequestException("Failed to send OTP.", null, HttpStatusCode.FailedDependency);
                }

                // Step 6: Save changes
                await _context.SaveChangesAsync();

                // Step 7: Return success response
                return ApiResponse<string>.SuccessResponse("OTP has been sent successfully to your email.");
            }
            catch
            {
                throw;
            }
        }
        private async Task<bool> SendOtpByEmailAsync(int otp, string emailAddress)
        {
            try
            {
                string body = $"Dear User,<br/><br/>Your OTP for Happinest App sign up is: {otp}<br/><br/>Best Regards,<br/>Happinest Team";

                await _emailService.SendEmailAsync(emailAddress, "OTP from Happinest", body);
                return true;

                //MailMessage message = new MailMessage();
                //SmtpClient smtp = new SmtpClient();
                //var Email = _configuration["SenderEmail"];
                //var Password = _configuration["SenderPassword"];
                //message.From = new MailAddress(Email);
                //message.To.Add(new MailAddress(emailAddress));
                //message.Subject = "";
                //message.IsBodyHtml = true; //to make message body as Html  
                //message.Body = "";
                //smtp.Port = Convert.ToInt32(_configuration["EmailPort"]);
                //smtp.Host = _configuration["Host"]; //for gmail host  
                //smtp.EnableSsl = Convert.ToBoolean(_configuration["EnableSsl"]);
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential(Email, Password);
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.Send(message);
                //return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CreateRandomPassword(int length = 8)
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string vali_happinestApiContexthars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = vali_happinestApiContexthars[random.Next(0, vali_happinestApiContexthars.Length)];
            }
            return new string(chars);
        }
        private async Task<bool> SendPasswordResetEmail(string sPassword, string emailAddress)
        {
            try
            {
                // Validate input email address
                if (string.IsNullOrWhiteSpace(emailAddress))
                    throw new ArgumentException("Email address is required.", nameof(emailAddress));

                // Retrieve the reset password email template from the database
                var template = await _context.EmailTemplateMasters
                                    .FirstOrDefaultAsync(x =>
                                        !string.IsNullOrWhiteSpace(x.TemplateCode) &&
                                        x.TemplateCode.ToLower() == EmailTemplateType.RESETPASSWORD.ToString().ToLower());

                // Check if the template was found
                if (template == null)
                    template = new EmailTemplateMaster
                    {
                        TemplateCode = EmailTemplateType.RESETPASSWORD.ToString(),
                        EmailSubject = "Your Happinest Password has been Reset",
                        EmailBody = @"Dear {{userName}},<br/><br/>
                                      Your password has been reset successfully. Your new password for the Happinest App is: 
                                      <strong>{{tempPassword}}</strong><br/><br/>
                                      Please login and change it at your earliest convenience.<br/><br/>
                                      Best Regards,<br/>Happinest Team",
                        EmailHeader = string.Empty,
                        EmailFooter = string.Empty
                    };

                // Ensure the template has required subject and body content
                if (string.IsNullOrWhiteSpace(template.EmailSubject) || string.IsNullOrWhiteSpace(template.EmailBody))
                    throw new InvalidOperationException("Reset password email template is incomplete (missing subject or body).");

                // Replace placeholders in the email body
                string emailBody = template.EmailBody ?? string.Empty;
                string bodyWithReplacements = emailBody
                                            .Replace("{{userName}}", emailAddress)
                                            .Replace("{{tempPassword}}", sPassword);

                // Build final email content
                string finalEmailBody = string.Empty;

                // Append header if available
                if (!string.IsNullOrWhiteSpace(template.EmailHeader))
                    finalEmailBody += template.EmailHeader;

                // Append main email content
                finalEmailBody += bodyWithReplacements;

                // Append footer if available
                if (!string.IsNullOrWhiteSpace(template.EmailFooter))
                    finalEmailBody += template.EmailFooter;

                // Send the email
                 await _emailService.SendEmailAsync(emailAddress, template.EmailSubject, finalEmailBody);
                return true;
            }
            catch
            {
                // Let the caller handle inner exceptions (preserves stack trace)
                throw;
            }
        }
        public async Task<BaseResponse> GenerateOtp(string email)
        {
            BaseResponse baseResponse = new BaseResponse();
            try
            {
                var userExist = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email == email.Trim());
                if (userExist != null)
                {
                    baseResponse.ResponseStatus = true;
                    baseResponse.ValidationMessage = "User already exist with " + email;
                    baseResponse.StatusCode = Constant.StatusCode.RecordAlreadyExist;
                    return baseResponse;
                }

                Random random = new Random();
                int genratedOtp = random.Next(100000, 999999);

                Otp existingOtp = await _context.Otps.Where(o => o.Email == email).FirstOrDefaultAsync();
                if (existingOtp != null)
                {
                    existingOtp.Otpcode = genratedOtp;
                    existingOtp.CreatedOn = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Otp otp = new Otp()
                    {
                        Email = email,
                        Otpcode = genratedOtp,
                        CreatedOn = DateTime.UtcNow,
                    };


                    await _context.AddAsync(otp);
                    await _context.SaveChangesAsync();
                }

                bool validationStatus = SendOtpByEmail(genratedOtp, email);
                if (validationStatus)
                {
                    baseResponse.ResponseStatus = true;
                    baseResponse.ValidationMessage = "Otp generated sucessfully. Check your email to get otp.";
                }
                return baseResponse;
            }
            catch (Exception)
            {
                throw;
            }

        }
        private bool SendOtpByEmail(int otp, string emailAddress)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                var Email = _configuration["SenderEmail"];
                var Password = _configuration["SenderPassword"];
                message.From = new MailAddress(Email);
                message.To.Add(new MailAddress(emailAddress));
                message.Subject = "OTP from traveloryapp.com";
                message.IsBodyHtml = true; //to make message body as Html  
                message.Body = "Dear User,<br/><br/> Your otp for Happinest App sign up is : " + otp.ToString() + " <br/><br/> Best Regards <br/> Happinest Team";
                smtp.Port = Convert.ToInt32(_configuration["EmailPort"]);
                smtp.Host = _configuration["Host"]; //for gmail host  
                smtp.EnableSsl = Convert.ToBoolean(_configuration["EnableSsl"]);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Email, Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AuthenticateResponse> GuestLogin()
        {
            try
            {
                if (_configuration == null)
                    throw new InvalidOperationException("Configuration service is not available.");

                if (_context == null)
                    throw new InvalidOperationException("Database context is not available.");

                if (_tokenService == null)
                    throw new InvalidOperationException("Token service is not available.");

                // Read expiry from config with fallback to 24 hours
                int tokenExpiryInHours = int.TryParse(
                                        _configuration["guestPermissions:tokenExpiryInHours"],
                                        out var hours
                                    ) ? hours : 24;


                string refreshToken = Guid.NewGuid().ToString();

                // Create session entry
                var guestSession = new GuestSession
                {
                    Token = string.Empty, // Will be updated after JWT is generated
                    RefreshToken = refreshToken,
                    CreatedOn = DateTime.UtcNow,
                    ExpiredOn = DateTime.UtcNow.AddHours(tokenExpiryInHours)
                };

                _context.GuestSessions.Add(guestSession);
                await _context.SaveChangesAsync();

                var randomGuestEmail = $"guest{Guid.NewGuid():N}@happinest.com";
                // Prepare claims
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", guestSession.Id.ToString()), // Match case with Login
                    new Claim(ClaimTypes.Email, randomGuestEmail),
                    new Claim(ClaimTypes.Role, UserRoles.Guest.ToString())
                };

                // Generate token
                string token = await _tokenService.GetToken(claims, tokenExpiryInHours);

                // Update session with generated token
                if (guestSession != null && guestSession.Id > 0)
                {
                    guestSession.Token = token;
                    _context.GuestSessions.Update(guestSession);
                    await _context.SaveChangesAsync();
                }

                // Get Guest role ID
                int guestRoleId = await _context.UserRoleMasters
                                .Where(x => string.Equals(x.RoleName, UserRoles.Guest.ToString()))
                                .Select(x => x.Id)
                                .FirstOrDefaultAsync();

                return new AuthenticateResponse
                {
                    FirstName = "Guest",
                    LastName = "guest",
                    Token = token,
                    Roles = new List<int> { guestRoleId },
                    ValidationMessage = "Success",
                    ResponseStatus = true,
                    StatusCode = StatusCode.Success,
                    GuestId = guestSession?.Id ?? 0
                };
            }
            catch (Exception ex)
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.Error,
                    ValidationMessage = ex?.InnerException?.Message ?? ex?.Message ?? "Something went wrong"
                };
            }
        }

        public async Task<AuthenticateResponse> Login(LoginRequestDto request)
        {
            try
            {
                var userDetail = await _context.AppUsers
                                    .Include(x => x.UserRoles)
                                    .FirstOrDefaultAsync(x => x.Email == request.Email && x.Status == (int)UserStatus.Active);

                // User not found or inactive
                if (userDetail == null)
                {
                    throw new KeyNotFoundException("User not found or inactive.");
                }

                // Validate password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, userDetail.Password))
                {
                    throw new UnauthorizedAccessException("Invalid password.");
                }

                /* Commented by Ram Sharma on 19-08-2025. dsicussed with Arpit*/

                //// Check existing session and device ID
                //bool isDeviceAllowed =
                //                    (!request.OverwriteExistingSession &&
                //                        (userDetail.DeviceId == request.DeviceId ||
                //                            userDetail.DeviceId == null ||
                //                            request.DeviceId == "UserPanel"))
                //                    || request.OverwriteExistingSession;

                //if ((userDetail.IsloggedIn ?? false) && !isDeviceAllowed)
                //{
                //    return new AuthenticateResponse
                //    {
                //        ResponseStatus = false,
                //        StatusCode = StatusCode.BadRequest,
                //        ValidationMessage = "You are already logged in on a different device. Please logout first or enable overwrite session."
                //    };
                //}

                /* 
                 * Maps the specified guest account to the existing user account.
                 * This ensures that any activity or session associated with the guest is linked
                 * to the permanent user once the user has signed up or logged in.
                */

                if (request.GuestId != null && request.GuestId > 0)
                {
                    await MapGuestToUserAsync(request.GuestId, userDetail.UserId);
                }
                // Proceed with login
                return await BuildAuthenticateResponse(userDetail, request.DeviceId);
            }
            catch (Exception)
            {
                // Optionally log the error
                throw;
            }
        }
        private async Task<AuthenticateResponse> BuildAuthenticateResponse(AppUser userDetail, string? deviceId)
        {

            var refreshTokenData = _tokenService.GenerateRefreshTokenWithExpiry();

            var response = new AuthenticateResponse
            {
                ServerUserId = userDetail.UserId,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Mobile = userDetail.Mobile,
                Email = userDetail.Email,
                Address = userDetail.Address,
                City = userDetail.City,
                State = userDetail.State,
                Gender = userDetail.Gender,
                CountryId = userDetail.CountryId,
                Preferences = userDetail.Preferences,
                AboutUser = userDetail.AboutUser,
                Status = userDetail.Status,
                IsloggedIn = true,
                IdentificationMark = userDetail.IdentificationMark,
                SignUpSource = userDetail.SignUpSource,
                RegistrationDate = userDetail.RegistrationDate,
                UserId = userDetail.UserId,
                Roles = userDetail.UserRoles?.Select(x => x.RoleId).ToList() ?? new List<int>(),
                UserProfilePictureUrl = !string.IsNullOrEmpty(userDetail.DisplayPicture)
                                        ? $"{_configuration["ImagesBaseUrl"]}/ProfilePicture/{userDetail.DisplayPicture}"
                                        : $"{_configuration["ImagesBaseUrl"]}/ProfilePicture/default.png",
                ResponseStatus = true,
                Token = await GetTokenAndClaims(userDetail.Email, userDetail.UserId),
                RefreshToken = refreshTokenData.RefreshToken ?? string.Empty,
                RefreshTokenExpiryTime = refreshTokenData.RefreshTokenExpiryTime
            };

            // Mark user as logged in
            userDetail.IsloggedIn = true;

            if (deviceId != "UserPanel")
            {
                userDetail.DeviceId = deviceId;
            }

            if (response != null)
            {
                if (!string.IsNullOrEmpty(response.RefreshToken))
                    userDetail.RefreshToken = response.RefreshToken;

                if (response.RefreshTokenExpiryTime.HasValue)
                    userDetail.RefreshTokenExpiryTime = response.RefreshTokenExpiryTime.Value;
            }

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<LogoutResonse> Logout()
        {
            long userId = _tokenService.GetUserId();
            LogoutResonse logoutResonse = new LogoutResonse();
            try
            {
                var resLogout = await (from objAppUser in _context.AppUsers
                                       where objAppUser.UserId == userId
                                       select objAppUser).FirstOrDefaultAsync();
                if (resLogout != null)
                {
                    resLogout.IsloggedIn = false;
                    resLogout.DeviceId = null;
                    await _context.SaveChangesAsync();
                }
                logoutResonse.UserId = userId;
                logoutResonse.IsloggedIn = false;
            }
            catch (Exception)
            {
                throw;
            }
            return logoutResonse;
        }

        public async Task<RefreshTokenReturnBaseDto> RefreshToken(CustomModels.RefreshRequest request)
        {
            try
            {
                string userEmail = _tokenService.GetUserEmailFromAccessToken(request.AccessToken);

                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new UnauthorizedAccessException("Invalid or expired access token.");
                }

                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    throw new KeyNotFoundException($"User with email '{userEmail}' not found.");
                }

                // Check if the provided refresh token matches the stored one
                if (user.RefreshToken != request.RefreshToken)
                {
                    throw new SecurityException("Invalid refresh token. Please login again.");
                }

                // Check if refresh token is expired
                if (user.RefreshTokenExpiryTime == null || DateTime.UtcNow >= user.RefreshTokenExpiryTime)
                {
                    throw new SecurityException("Refresh token has expired. Please login again.");
                }

                // Generate new refresh token
                var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshTokenWithExpiry();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshTokenExpiry;

                await _context.SaveChangesAsync();

                return new RefreshTokenReturnBaseDto()
                {
                    Data = new RefreshTokenReturnDto()
                    {
                        Token = await GetTokenAndClaims(user.Email, user.UserId),
                        RefreshToken = user.RefreshToken ?? string.Empty,
                        RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
                    },
                    ResponseStatus = true,
                    StatusCode = StatusCode.Success,
                    ValidationMessage = "Success"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<AuthenticateResponse> SocialLoginAsync(SocialLoginRequest request)
        {
            try
            {
                // Try to parse the authentication source from the request (e.g., "Meta", "Google")
                Enum.TryParse<AuthenticationSource>(request.AuthenticationSource, out AuthenticationSource authSource);

                return authSource switch
                {
                    AuthenticationSource.Meta => await HandleMetaLoginAsync(request.AccessToken, request.DeviceId, request.GuestId),

                    AuthenticationSource.Google => await GoogleLoginAsync(request.AccessToken, request.DeviceId, request.GuestId),

                    _ => new AuthenticateResponse
                    {
                        ResponseStatus = false,
                        StatusCode = StatusCode.Invalid,
                        ValidationMessage = $"Unsupported authentication source: {request.AuthenticationSource}"
                    },
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Social login failed.", ex);
            }
        }
        private async Task<AuthenticateResponse> HandleMetaLoginAsync(string accessToken, string? deviceId, int? guestId = 0)
        {
            var fbAppId = _configuration["Meta:appId"];
            var fbAppSecret = _configuration["Meta:appSecret"];
            var baseUrl = _configuration["Meta:baseUrl"];
            var tokenValidationTemplate = _configuration["Meta:tokenValidationUrl"];
            var userInfoTemplate = _configuration["Meta:userInfoUrl"];

            // Validate required values
            if (string.IsNullOrWhiteSpace(fbAppId) ||
                string.IsNullOrWhiteSpace(fbAppSecret) ||
                string.IsNullOrWhiteSpace(baseUrl) ||
                string.IsNullOrWhiteSpace(tokenValidationTemplate) ||
                string.IsNullOrWhiteSpace(userInfoTemplate) ||
                string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidOperationException("One or more required Meta configuration values are missing.");
            }

            // Step 1: Build the Meta token validation URL
            var tokenValidationUrl = BuildFacebookGraphUrl(tokenValidationTemplate, baseUrl, accessToken, fbAppId, fbAppSecret);

            // Step 2: Make the HTTP request to validate the token
            var tokenValidationResponse = await _httpService.GetAsync<FacebookTokenDebugResponse>(tokenValidationUrl);

            // Step 3: Handle possible error from Meta
            if (!string.IsNullOrWhiteSpace(tokenValidationResponse?.Error?.Message))
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.Unauthorized,
                    ValidationMessage = tokenValidationResponse.Error.Message
                };
            }

            // Step 4: Check if the token is invalid
            if (tokenValidationResponse?.Data?.IsValid != true)
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.Unauthorized,
                    ValidationMessage = "Invalid meta token"
                };
            }

            // Step 2: Get user information from Meta
            // Build user info URL
            var userInfoUrl = BuildFacebookGraphUrl(userInfoTemplate, baseUrl, accessToken);

            var userInfoResponse = await _httpService.GetAsync<FacebookUser>(userInfoUrl);

            // Step 3: Validate required fields
            if (string.IsNullOrWhiteSpace(userInfoResponse?.Email))
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.Invalid,
                    ValidationMessage = "Meta account does not have a valid email address"
                };
            }

            // Step 4: Try to find existing user by email
            var userDetail = await GetUserByEmailAsync(userInfoResponse.Email);

            // Step 5: If user does not exist, register a new user
            if (userDetail == null)
            {
                var newUserResponse = await AddNewUser(
                    email: userInfoResponse.Email,
                    password: string.Empty,
                    source: AuthenticationSource.Meta.ToString(),
                    deviceId: deviceId ?? string.Empty,
                    appleUserId: string.Empty,
                    firstName: string.Empty, lastName: string.Empty  , fullName: string.Empty   
                );

                if (newUserResponse?.ResponseStatus == true || newUserResponse?.StatusCode == StatusCode.UserNameExist)
                {
                    userDetail = await GetUserByEmailAsync(newUserResponse.Email);
                }
            }

            // Step 6: If user still not found after attempted registration
            if (userDetail == null)
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.NotFound,
                    ValidationMessage = "User could not be created or found"
                };
            }

            await MapGuestToUserAsync(guestId, userDetail.UserId);
            // Step 7: Build and return authentication pagedEvents (with JWT)
            return await BuildAuthenticateResponse(userDetail, string.Empty);
        }
        private async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _context.AppUsers
                .Include(x => x.UserRoles) // Include user roles for authentication and authorization logic
                .FirstOrDefaultAsync(x => x.Email == email && x.Status == (int)UserStatus.Active); // Ensure user is active
        }
        private string BuildFacebookGraphUrl(string template, string baseUrl, string accessToken, string? appId = null, string? appSecret = null)
        {
            return template
                .Replace("{BaseUrl}", baseUrl)
                .Replace("{AccessToken}", accessToken)
                .Replace("{AppId}", appId ?? string.Empty)
                .Replace("{AppSecret}", appSecret ?? string.Empty);
        }
        public async Task<AuthenticateResponse> GoogleLoginAsync(string idToken, string? deviceId, int? guestId = 0)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                // Validate the Google ID token and extract payload (user info like email, name, etc.)
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch
            {
                // Return unauthorized pagedEvents if token validation fails
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.Unauthorized,
                    ValidationMessage = "Invalid Google token"
                };
            }

            // If the payload does not contain an email, treat it as an invalid token
            if (string.IsNullOrWhiteSpace(payload.Email))
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.BadRequest,
                    ValidationMessage = "Google token does not contain a valid email address"
                };
            }

            // Try to fetch existing user by email
            var userDetail = await GetUserByEmailAsync(payload.Email);

            if (userDetail == null)
            {
                // If user doesn't exist, register them using AddNewUser method
                var newUserResponse = await AddNewUser(
                                        email: payload.Email,
                                        password: string.Empty, // No password required for Google auth
                                        source: AuthenticationSource.Google.ToString(),
                                        deviceId: deviceId ?? string.Empty,
                                        appleUserId: string.Empty,
                                        firstName: string.Empty,
                                        lastName: string.Empty,
                                        fullName: string.Empty
                                    );

                // Fetch the newly created user from DB if registration succeeded
                if (newUserResponse?.ResponseStatus == true || newUserResponse?.StatusCode == StatusCode.UserNameExist)
                {
                    userDetail = await GetUserByEmailAsync(newUserResponse.Email);
                }
            }

            // If user still not found after registration, return failure pagedEvents
            if (userDetail == null)
            {
                return new AuthenticateResponse
                {
                    ResponseStatus = false,
                    StatusCode = StatusCode.NotFound,
                    ValidationMessage = "Invalid user"
                };
            }

            await MapGuestToUserAsync(guestId, userDetail.UserId);

            // Return successful authentication pagedEvents
            return await BuildAuthenticateResponse(userDetail, string.Empty);
        }

        public async Task<BaseResponse> UpdateDeviceToken(DeviceTokenRequest tokenRequest)
        {
            BaseResponse baseResponse = new BaseResponse();
            try
            {
                long userId = _tokenService.GetUserId();

                var resAppUser = await (from obj in _context.AppUsers
                                        where obj.UserId == userId
                                        select obj).FirstOrDefaultAsync();
                if (resAppUser != null)
                {
                    resAppUser.DeviceToken = tokenRequest.Token;
                    await _context.SaveChangesAsync();

                    baseResponse.ResponseStatus = true;
                    baseResponse.StatusCode = Constant.StatusCode.Success;
                    baseResponse.ValidationMessage = "Success";
                }
                else
                {
                    baseResponse.ResponseStatus = false;
                    baseResponse.StatusCode = Constant.StatusCode.Failed;
                    baseResponse.ValidationMessage = "Failed";
                }
            }
            catch
            {
                throw;
            }
            return baseResponse;
        }

        public async Task<ApiResponse<string>> ValidateOTP(ValidateOTP request)
        {
            try
            {
                // Step 1: Validate input
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new HttpRequestException("Email is required.", null, HttpStatusCode.BadRequest);
                }

                // Step 2: Fetch user
                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Status == (int)UserStatus.Active);

                if (user == null)
                {
                    return ApiResponse<string>.ErrorResponse(HttpStatusCode.BadRequest, "Invalid email address.");

                }

                // Step 3: Fetch OTP record
                var otpEntity = await _context.Otps.FirstOrDefaultAsync(o => o.Email == user.Email);

                if (otpEntity == null)
                {
                    return ApiResponse<string>.ErrorResponse(HttpStatusCode.BadRequest, "No OTP found for this email. Please request a new one.");
                }

                // Step 4: Validate OTP
                if (!await ValidateOtp(user.Email, request.Otp))
                {
                    return ApiResponse<string>.ErrorResponse(HttpStatusCode.BadRequest, "Invalid or expired OTP.");

                }

                // Step 5: Return success
                return ApiResponse<string>.SuccessResponse("OTP Validate Successfully.");
            }
            catch (HttpRequestException)
            {
                throw; // Allow middleware/global handler to return a formatted HTTP response
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.InnerException?.Message ?? ex.Message ?? "An unexpected error occurred while resetting the password.",
                                                ex, HttpStatusCode.InternalServerError);
            }
        }
    }
}
