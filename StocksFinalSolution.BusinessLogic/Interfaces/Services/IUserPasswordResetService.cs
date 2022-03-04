using System.Threading.Tasks;
using StocksProcessing.API.Auth.Dtos;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication;

public interface IUserPasswordResetService
{
    Task ForgotPasswordRequest(ModifyPasswordRequest request,
        string frontEndUrlResetPasswordUrl);

    Task ResetPassword(ResetPasswordRequest request);
}