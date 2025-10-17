using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Happinest.Services.AuthAPI.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Happinest.Services.AuthAPI.Services
{
    public class HttpService : IHttpService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        public HttpService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient()
            {
                MaxResponseContentBufferSize = int.MaxValue,
                Timeout = TimeSpan.FromHours(1)

            };
        }

        public void AddBearerAuthorization(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RequestURL"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string RequestURL)
        {
            HttpResponseMessage httpResponse = await _client.GetAsync(RequestURL);
            return await ConvertAsync<T>(httpResponse);
        }

        public async Task<string> PostAsync<TIn>(string url, TIn model)
        {
            using var client = new HttpClient { BaseAddress = new Uri(url) };

            // Post JSON asynchronously
            using var httpResponseMessage = await client.PostAsJsonAsync("", model);

            // Read content asynchronously
            string result = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                throw new HttpRequestException($"HTTP request failed with status code {httpResponseMessage.StatusCode}: {result}");
            }
        }
        public void AddHeader(string key, string value)
        {
            if (!_client.DefaultRequestHeaders.Contains(key))
            {
                _client.DefaultRequestHeaders.Add(key, value);
            }
        }

        private async Task<T> ConvertAsync<T>(HttpResponseMessage httpResponse)
        {
            T? ResponseObject = default;

            string JSON_Data = await httpResponse.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            ResponseObject = JsonConvert.DeserializeObject<T>(JSON_Data, settings);

            return ResponseObject!;
        }

        public async Task<string> ConvertImageUrlToBase64Async(string imageUrl)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(imageUrl);

                    if (!response.IsSuccessStatusCode)
                        return string.Empty;

                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var base64String = Convert.ToBase64String(imageBytes);

                    return $"data:{contentType};base64,{base64String}";
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
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

        public async Task<ReturnType> PostAsync<ReturnType, PayloadType>(string RequestURL, PayloadType Payload)
        {
            HttpResponseMessage httpResponse;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string JSON = JsonConvert.SerializeObject(Payload, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                var content = new StringContent(JSON, Encoding.UTF8, "application/json");
                httpResponse = await _client.PostAsync(RequestURL, content);
            }
            catch
            {
                throw;
            }
            return await ConvertAsync<ReturnType>(httpResponse);
        }
    }
}
