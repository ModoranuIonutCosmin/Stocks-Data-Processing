namespace StocksProcessing.API.Auth.Dtos
{
    public class RegisterUserDataModelRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
