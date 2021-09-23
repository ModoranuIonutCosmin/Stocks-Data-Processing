using System.ComponentModel.DataAnnotations;

namespace StocksProcessing.API.Auth.Dtos
{
    public class ModifyPasswordRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

    }
}
