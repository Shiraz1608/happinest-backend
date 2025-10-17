using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class FacebookUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public PictureData Picture { get; set; }

    }

    public class PictureData
    {
        public Picture Picture { get; set; }
    }

    public class Picture
    {
        public PictureDetails Data { get; set; }
    }

    public class PictureDetails
    {
        public string Url { get; set; }
    }

    public class FacebookTokenDebugResponse
    {
        [JsonProperty("data")]
        public FacebookTokenData Data { get; set; }

        [JsonProperty("error")]
        public FacebookError Error { get; set; }
    }

    public class FacebookTokenData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("data_access_expires_at")]
        public int? DataAccessExpiresAt { get; set; }

        [JsonProperty("expires_at")]
        public int? ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class FacebookError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("fbtrace_id")]
        public string FbtraceId { get; set; }
    }
}
