namespace StocksProccesing.Relational.Email
{
    public class SendEmailResponse
    {
        public bool Successful => ErrorMessage == null;
        public string ErrorMessage { get; set; }
    }
}
