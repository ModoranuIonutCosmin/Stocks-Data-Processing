using StocksProccesing.Relational.Model;
using StocksProcessing.API.Email.Dtos;
using StocksProcessing.API.Email.Interfaces;
using System.Threading.Tasks;

namespace StocksProcessing.API.Email
{
    public class GeneralPurposeEmailService : IGeneralPurposeEmailService
    {
        private readonly ITemplatedEmailSender templatedEmailSender;

        public GeneralPurposeEmailService(ITemplatedEmailSender templatedEmailSender,
            IDirectEmailSender directEmailSender)
        {
            this.templatedEmailSender = templatedEmailSender;
        }

        public async Task<SendEmailResponse> SendConfirmationEmail(ApplicationUser recipient,
            string confirmationLink)
        {
            var emailDetails = new SendEmailDetails
            {
                FromEmail = "accounts@modsdeal.com",
                ToEmail = recipient.Email,
                ToName = recipient.FirstName,
                Subject = "Email Confirmation for ModsDeal",
            };

            return await templatedEmailSender
                .SendEmailAsync(emailDetails, "message",
                new()
                {
                    { "--Username--", recipient.UserName },
                    { "--Content1--", "An attempt to register an account with your email was made." },
                    {
                        "--Content2--",
                        "If it was made by you, click the button below to confirm email " +
                        "otherwise, simply ignore this message"
                    },
                    { "--Link--", confirmationLink },
                    { "--LinkAction--", "Confirm" }
                });
        }

    }
}
