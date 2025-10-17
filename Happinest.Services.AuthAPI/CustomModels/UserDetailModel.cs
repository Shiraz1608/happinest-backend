namespace Happinest.Services.AuthAPI.CustomModels
{
    public class UserDetailModel : BaseResponse
    {
        public long UserId { get; set; }
        public long ServerUserId { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string CountryFlag { get; set; }
        public List<string> Preferences { get; set; }
        public string AboutUser { get; set; }
        public int? TravelTypeId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? Status { get; set; }
        public bool IsloggedIn { get; set; }
        public string IdentificationMark { get; set; }
        public string SignUpSource { get; set; }
        //public string ValidationMessage { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public long RunningTripId { get; set; }
        public string RunningTripName { get; set; }
        public int MinDistanceForLocationTracking { get; set; }
        public int MinDurationForLocationTracking { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public string UserBackgroundPictureUrl { get; set; }
        public string ErrorResponse { get; set; }
        public string Token { get; set; }
        public bool IsAlreadySignedUp { get; set; }
        public string DeviceId { get; set; }
        public bool OverWriteExistingSession { get; set; }
        public string AuthenticationSource { get; set; }
        public string AppleUserId { get; set; }
        public bool ToolTipsVisited { get; set; }
        public int EventsCount { get; set; }
        public int VideosCount { get; set; }
    }
    public class GetUserAndTripsResponse
    {
        public long UserId { get; set; }
        public long ServerUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; }
        public string FoodPreference { get; set; }
        public string AboutUser { get; set; }
        public int? TravelTypeId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime Birthday { get; set; }
        public bool? Status { get; set; }
        public bool IsloggedIn { get; set; }
        public string IdentificationMark { get; set; }
        public string SignUpSource { get; set; }
        public string ValidationMessage { get; set; }
        public long FollowersCount { get; set; }
        public int TripCount { get; set; }
        public int DayCount { get; set; }
        public int CityCount { get; set; }
        public long Miles { get; set; }
        public long Stairs { get; set; }
        public long Steps { get; set; }
        public long FollowingCount { get; set; }
        public long RunningTrip { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public string UserBackgroundPictureUrl { get; set; }
        public string CountryFlagUrl { get; set; }
        public string CountryImageUrl { get; set; }
        public FollowerStatus FollowerStatus { get; set; }

    }

    public enum CoAuthorAccessType
    {
        R = 1,
        W = 2
    }

    public enum FollowerStatus
    {
        Following = 1,
        NotFollowing = 2,
        FollowRequestPending = 3
    }

    public enum FollowingStatus
    {
        Following = 1,
        NotFollowing = 2,
        FollowingRequestPending = 3
    }

    public enum CoAuthorAccessLevel
    {
        ReadOnly = 'R',
        ReadWrite = 'W'
    }
    public class LogoutResonse
    {
        public long UserId { get; set; }
        public bool IsloggedIn { get; set; }
    }

    public enum ActionBy
    {
        Owner = 1,
        CoAuthor = 2
    }
}
