namespace Happinest.Services.AuthAPI.CustomModels
{
    public class AuthenticateResponse : BaseResponse
    {
        public long UserId { get; set; }
        public long ServerUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }
        public int? CountryId { get; set; }
        public string Preferences { get; set; }
        public string AboutUser { get; set; }
        public int? TravelTypeId { get; set; }
        public int? Status { get; set; }
        public bool IsloggedIn { get; set; }
        public string IdentificationMark { get; set; }
        public string SignUpSource { get; set; }
        public DateTime RegistrationDate { get; set; }
        //public long RunningTripId { get; set; }
        //public string RunningTripName { get; set; }
        //public int MinDistanceForLocationTracking { get; set; }
        //public int MinDurationForLocationTracking { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public List<int>? Roles { get; set; }

        public int? GuestId { get; set; }
    }
}
