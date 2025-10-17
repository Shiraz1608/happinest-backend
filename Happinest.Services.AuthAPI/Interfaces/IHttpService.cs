namespace Happinest.Services.AuthAPI.Interfaces
{
    public interface IHttpService
    {
        Task<string> PostAsync<TIn>(string url, TIn model);
        /// <summary>
        /// Get method
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="RequestURL"></param>
        /// <returns></returns>
        Task<ReturnType> GetAsync<ReturnType>(string RequestURL);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        Task<string> ConvertImageUrlToBase64Async(string imageUrl);
        /// <summary>
        /// Sends an asynchronous HTTP POST request with a JSON payload to the specified URL
        /// and deserializes the response into the specified <typeparamref name="ReturnType"/>.
        /// </summary>
        /// <typeparam name="ReturnType">The type to deserialize the HTTP response into.</typeparam>
        /// <typeparam name="PayloadType">The type of the payload object to send in the request body.</typeparam>
        /// <param name="RequestURL">The full URL to which the POST request is sent.</param>
        /// <param name="Payload">The payload object to serialize as JSON and include in the request body.</param>
        /// <returns>A <see cref="Task{ReturnType}"/> representing the asynchronous operation, 
        /// containing the deserialized response of type <typeparamref name="ReturnType"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
        /// <exception cref="JsonException">Thrown if serialization or deserialization fails.</exception>
        Task<ReturnType> PostAsync<ReturnType, PayloadType>(string RequestURL, PayloadType Payload);

        void AddHeader(string key, string value);

        void AddBearerAuthorization(string token);

    }
}
