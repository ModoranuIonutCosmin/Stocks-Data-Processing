using System.Threading.Tasks;
using StocksProccesing.Relational.Email;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Email
{
    public interface IGeneralPurposeEmailService
    {
        Task<SendEmailResponse> SendConfirmationEmail(ApplicationUser recipient,
            string confirmationLink);

        Task<SendEmailResponse> SendResetPasswordEmail(ApplicationUser recipient,
            string resetPasswordLink);
    }
}
