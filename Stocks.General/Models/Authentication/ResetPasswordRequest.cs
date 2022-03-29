using System.ComponentModel.DataAnnotations;

namespace Stocks.General.Models.Authentication;

public class ResetPasswordRequest
{
    [EmailAddress] [Required] public string Email { get; set; }

    public string Token { get; set; }

    public string NewPassword { get; set; }
}