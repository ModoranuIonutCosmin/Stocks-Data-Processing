using System.Threading.Tasks;
using StocksProcessing.API.Auth.Dtos;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication;

public interface IUserAuthenticationService
{
    Task<RegisterUserDataModelResponse> RegisterAsync(
        RegisterUserDataModelRequest registerData,
        string frontEndUrl);

    Task<UserProfileDetailsApiModel> LoginAsync(LoginUserDataModel loginData,
        string jwtSecret, string issuer, string audience);

    Task ConfirmEmail(string email, string confirmationToken);
}