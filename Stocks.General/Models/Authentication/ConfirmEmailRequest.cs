namespace Stocks.General.Models.Authentication;

public class ConfirmEmailRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}