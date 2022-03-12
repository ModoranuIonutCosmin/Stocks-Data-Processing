using System.Threading.Tasks;
using Stocks.General.Models.Authentication;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication;

public interface IUserPasswordResetService
{
    Task ForgotPasswordRequest(ModifyPasswordRequest request,
        string frontEndUrlResetPasswordUrl);

    Task ResetPassword(ResetPasswordRequest request);
}