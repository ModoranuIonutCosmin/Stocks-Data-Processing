namespace StocksProcessing.API.Email
{
    public class SendEmailResponse
    {
        public bool Successful => ErrorMessage == null;
        public string ErrorMessage { get; set; }
    }
}
