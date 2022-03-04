using System.Threading.Tasks;
using StocksProccesing.Relational.Email;
using StocksProccesing.Relational.Email.Dtos;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Email
{
    public interface IDirectEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails);
    }
}
