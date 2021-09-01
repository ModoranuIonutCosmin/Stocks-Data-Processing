using StocksProcessing.API.Email.Dtos;
using System.Threading.Tasks;

namespace StocksProcessing.API.Email.Interfaces
{
    public interface IDirectEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails);
    }
}
