using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class ForgotPasswordRequest
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
