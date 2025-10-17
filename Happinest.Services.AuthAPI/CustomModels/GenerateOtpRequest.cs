using System.ComponentModel.DataAnnotations;

namespace Happinest.Services.AuthAPI.CustomModels
{
    public class GenerateOtpRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
