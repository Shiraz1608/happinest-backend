using Happinest.Services.AuthAPI.CustomModels.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Happinest.Services.AuthAPI.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Returns an appropriate IActionResult based on the ApiResponse status.
        /// </summary>
        /// <typeparam name="T">Type of data in the response.</typeparam>
        /// <param name="response">The ApiResponse object containing result or error.</param>
        /// <returns>IActionResult with proper HTTP status code.</returns>
        protected IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            if (response is null)
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ApiResponse<string>.ErrorResponse(HttpStatusCode.InternalServerError, "Response is null"));

            if (response.Success)
                return Ok(response);

            return response.Error?.Code switch
            {
                HttpStatusCode.BadRequest => BadRequest(response),
                HttpStatusCode.Unauthorized => Unauthorized(response),
                HttpStatusCode.Forbidden => StatusCode((int)HttpStatusCode.Forbidden, response),
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.Conflict => Conflict(response),
                HttpStatusCode.Gone => StatusCode((int)HttpStatusCode.Gone, response),
                HttpStatusCode.PreconditionFailed => StatusCode((int)HttpStatusCode.PreconditionFailed, response),
                HttpStatusCode.RequestTimeout => StatusCode((int)HttpStatusCode.RequestTimeout, response),
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                HttpStatusCode.NotImplemented => StatusCode((int)HttpStatusCode.NotImplemented, response),
                HttpStatusCode.ServiceUnavailable => StatusCode((int)HttpStatusCode.ServiceUnavailable, response),
                HttpStatusCode.GatewayTimeout => StatusCode((int)HttpStatusCode.GatewayTimeout, response),
                HttpStatusCode.Accepted => Accepted(response),
                HttpStatusCode.Created => StatusCode((int)HttpStatusCode.Created, response),
                HttpStatusCode.NoContent => NoContent(),
                _ => StatusCode((int)(response.Error?.Code ?? HttpStatusCode.InternalServerError), response)
            };
        }

        /// <summary>
        /// Shortcut for returning 200 OK with data.
        /// </summary>
        protected IActionResult OkResponse<T>(T data)
        {
            var response = ApiResponse<T>.SuccessResponse(data);
            return Ok(response);
        }

        /// <summary>
        /// Shortcut for returning an error response with a given code and message.
        /// </summary>
        protected IActionResult ErrorResponse(HttpStatusCode statusCode, string message, string? devMessage = null)
        {
            var response = ApiResponse<string>.ErrorResponse(statusCode, message, devMessage);
            return StatusCode((int)statusCode, response);
        }
    }
}
