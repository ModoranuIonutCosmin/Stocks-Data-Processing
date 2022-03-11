using System.Threading.Tasks;
using Stocks.General.Models;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Email
{
    public interface IDirectEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails);
    }
}
