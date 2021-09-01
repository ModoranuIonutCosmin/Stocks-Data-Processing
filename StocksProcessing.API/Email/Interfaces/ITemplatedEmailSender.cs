using StocksProcessing.API.Email.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StocksProcessing.API.Email.Interfaces
{
    public interface ITemplatedEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails,
            string templateName, Dictionary<string, string> substitutions);
    }
}
