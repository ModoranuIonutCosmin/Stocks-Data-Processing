using StocksProccesing.Relational.Model;
using System.Threading.Tasks;

namespace StocksProcessing.API.Email.Interfaces
{
    public interface IGeneralPurposeEmailService
    {
        Task<SendEmailResponse> SendConfirmationEmail(ApplicationUser recipient,
            string confirmationLink);

        Task<SendEmailResponse> SendResetPasswordEmail(ApplicationUser recipient,
            string resetPasswordLink);
    }
}
