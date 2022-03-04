using System.Collections.Generic;
using System.Threading.Tasks;
using StocksProccesing.Relational.Email;
using StocksProccesing.Relational.Email.Dtos;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Email
{
    public interface ITemplatedEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails,
            string templateName, Dictionary<string, string> substitutions);
    }
}
