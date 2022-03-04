namespace StocksProcessing.API.Auth.Dtos
{
    public class LoginUserDataModel
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }
    }
}
