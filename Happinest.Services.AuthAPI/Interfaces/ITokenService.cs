using System.Security.Claims;
using static Happinest.Services.AuthAPI.Helpers.Constant;

namespace Happinest.Services.AuthAPI.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token with the specified claims and expiry hours.
        /// </summary>
        /// <param name="claims">The claims to include in the token.</param>
        /// <param name="hours">Optional token expiry in hours. Defaults to configuration value or 24 hours if invalid.</param>
        /// <returns>The generated JWT token as a string.</returns>
        Task<string> GetToken(List<Claim> claims, int hours = 0);
        long GetUserId();
        /// <summary>
        /// Retrieves the current user's role from the HTTP context.
        /// </summary>
        /// <remarks>
        /// - If the role claim is missing, defaults to <see cref="UserRoles.Guest"/>.  
        /// - If the claim value matches a valid <see cref="UserRoles"/> enum, returns the corresponding role.  
        /// - Throws <see cref="UnauthorizedAccessException"/> if the role claim exists but is invalid.  
        /// </remarks>
        /// <returns>The user's role as a <see cref="UserRoles"/> enum.</returns>
        UserRoles GetUserRole();
        /// <summary>
        /// Extracts the user's email from a JWT access token.
        /// Throws an exception if the token is invalid or does not contain the email claim.
        /// </summary>
        /// <param name="accessToken">The JWT access token to validate.</param>
        /// <returns>The user's email.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="accessToken"/> is null or empty.</exception>
        /// <exception cref="SecurityTokenException">Thrown if the token is invalid or cannot be validated.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the token does not contain an email claim.</exception>
        string GetUserEmailFromAccessToken(string accessToken);
        /// <summary>
        /// Generates a new refresh token and calculates its expiry based on configuration.
        /// </summary>
        /// <returns>Tuple containing RefreshToken and RefreshTokenExpiryTime</returns>
        public (string RefreshToken, DateTime RefreshTokenExpiryTime) GenerateRefreshTokenWithExpiry();
    }
}
