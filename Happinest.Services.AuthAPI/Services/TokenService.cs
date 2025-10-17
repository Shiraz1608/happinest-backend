using Happinest.Services.AuthAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Happinest.Services.AuthAPI.Helpers.Constant;

namespace Happinest.Services.AuthAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Generates a JWT token with the specified claims and expiry hours.
        /// </summary>
        /// <param name="claims">The claims to include in the token.</param>
        /// <param name="hours">Optional token expiry in hours. Defaults to configuration value or 24 hours if invalid.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public async Task<string> GetToken(List<Claim> claims, int hours = 0)
        {
            // Determine expiry hours
            if (hours <= 0)
            {
                hours = int.TryParse(_configuration["JwtToken:tokenExpiry"], out var expiry) ? expiry : 24;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtToken:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtToken:Issuer"],
                audience: _configuration["JwtToken:Audiance"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(hours),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public long GetUserId()
        {
            // Check for guest user by email claim
            var emailClaim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
            if (!string.IsNullOrEmpty(emailClaim) && emailClaim.StartsWith("guest", StringComparison.OrdinalIgnoreCase))
            {
                return 0; // Indicate guest/system user
            }

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim != null)
            {
                return Convert.ToInt64(userIdClaim.Value);
            }

            throw new UnauthorizedAccessException("User is unauthorized.");
        }

        /// <summary>
        /// Retrieves the current user's role from the HTTP context.
        /// </summary>
        /// <remarks>
        /// - If the role claim is missing, defaults to <see cref="UserRoles.Guest"/>.  
        /// - If the claim value matches a valid <see cref="UserRoles"/> enum, returns the corresponding role.  
        /// - Throws <see cref="UnauthorizedAccessException"/> if the role claim exists but is invalid.  
        /// </remarks>
        /// <returns>The user's role as a <see cref="UserRoles"/> enum.</returns>
        public UserRoles GetUserRole()
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // If no role claim is found, treat as guest
            if (string.IsNullOrEmpty(roleClaim))
            {
                return UserRoles.Guest;
            }

            if (Enum.TryParse<UserRoles>(roleClaim, true, out var role))
            {
                return role;
            }

            throw new UnauthorizedAccessException("User role is missing or unauthorized.");
        }

        /// <summary>
        /// Extracts the user's email from a JWT access token.
        /// Throws an exception if the token is invalid or does not contain the email claim.
        /// </summary>
        /// <param name="accessToken">The JWT access token to validate.</param>
        /// <returns>The user's email.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="accessToken"/> is null or empty.</exception>
        /// <exception cref="SecurityTokenException">Thrown if the token is invalid or cannot be validated.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the token does not contain an email claim.</exception>
        public string GetUserEmailFromAccessToken(string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new ArgumentNullException(nameof(accessToken), "Access token cannot be null or empty.");

                var key = Encoding.ASCII.GetBytes(_configuration["JwtToken:Key"]);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = false, // Lifetime is not validated here
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                SecurityToken securityToken;
                ClaimsPrincipal principal;
                try
                {
                    principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
                }
                catch (Exception ex)
                {
                    throw new SecurityTokenException("Invalid or malformed access token.", ex);
                }

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm.");
                }

                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(userEmail))
                    throw new InvalidOperationException("Token does not contain an email claim.");

                return userEmail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Generates a new refresh token and calculates its expiry based on configuration.
        /// </summary>
        /// <returns>Tuple containing RefreshToken and RefreshTokenExpiryTime</returns>
        public (string RefreshToken, DateTime RefreshTokenExpiryTime) GenerateRefreshTokenWithExpiry()
        {
            string refreshToken = GenerateRefreshToken();
            double expiryHours = 24; // default

            if (double.TryParse(_configuration["JwtToken:RefreshTokenExpiryTime"], out double configuredHours))
            {
                expiryHours = configuredHours;
            }

            DateTime expiryTime = DateTime.UtcNow.AddHours(expiryHours);
            return (refreshToken, expiryTime);
        }

        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
