using System.Net;

namespace Happinest.Services.AuthAPI.CustomModels.SharedModels
{
    /// <summary>
    /// Represents a standard API response with generic data and error details.
    /// Immutable and created through factory methods.
    /// </summary>
    /// <typeparam name="T">Type of the response data.</typeparam>
    public sealed record ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The data payload for successful responses.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Error details if the request failed.
        /// </summary>
        public ApiError? Error { get; init; }

        /// <summary>
        /// Creates a success response with the provided data.
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data) => new ApiResponse<T> { Success = true, Data = data };

        /// <summary>
        /// Creates an error response with the provided HTTP status code and message.
        /// </summary>
        public static ApiResponse<T> ErrorResponse(HttpStatusCode code, string message, string? developerMessage = null) =>
            new ApiResponse<T>
            {
                Success = false,
                Error = new ApiError(code, message, developerMessage)
            };
    }

    /// <summary>
    /// Represents details of an error returned by an API.
    /// Immutable and can only be created through constructor.
    /// </summary>
    public sealed record ApiError(HttpStatusCode Code, string Message, string? DeveloperMessage = null);
}
