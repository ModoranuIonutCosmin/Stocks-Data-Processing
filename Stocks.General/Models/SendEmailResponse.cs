namespace Stocks.General.Models;

public class SendEmailResponse
{
    public bool Successful => ErrorMessage == null;
    public string ErrorMessage { get; set; }
}