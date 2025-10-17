using Microsoft.AspNetCore.Http;
using static Happinest.Services.AuthAPI.Helpers.Constant;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class BaseResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool ResponseStatus { get; set; }

        /// <summary>
        /// Gets or sets the validation or status message for the operation.
        /// </summary>
        public string ValidationMessage { get; set; }

        /// <summary>
        /// Gets or sets the status code indicating the result of the operation.
        /// </summary>
        public StatusCode StatusCode { get; set; }
    }
}
