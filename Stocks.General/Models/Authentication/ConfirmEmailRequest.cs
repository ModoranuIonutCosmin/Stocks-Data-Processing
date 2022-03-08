namespace StocksProcessing.API.Auth.Dtos;

public class ConfirmEmailRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}