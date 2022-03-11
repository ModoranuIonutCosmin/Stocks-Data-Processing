using System.ComponentModel.DataAnnotations;

namespace Stocks.General.Models.Authentication
{
    public class ModifyPasswordRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

    }
}
