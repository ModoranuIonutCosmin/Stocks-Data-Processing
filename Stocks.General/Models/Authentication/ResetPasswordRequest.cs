using System.ComponentModel.DataAnnotations;

namespace StocksProcessing.API.Auth.Dtos
{
    public class ResetPasswordRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public string Token {  get; set; }

        public string NewPassword {  get; set; }
    }
}
