using System.Threading.Tasks;
using Stocks.General.Models.Authentication;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services;

public interface IUserPasswordResetService
{
    Task ForgotPasswordRequest(ModifyPasswordRequest request,
        string frontEndUrlResetPasswordUrl);

    Task ResetPassword(ResetPasswordRequest request);
}