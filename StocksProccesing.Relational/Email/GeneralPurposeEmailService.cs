using System.Threading.Tasks;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.Interfaces.Email;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.Email

{
    public class GeneralPurposeEmailService : IGeneralPurposeEmailService
    {
        private readonly ITemplatedEmailSender templatedEmailSender;

        public GeneralPurposeEmailService(ITemplatedEmailSender templatedEmailSender
            )
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


        public async Task<SendEmailResponse> SendResetPasswordEmail(ApplicationUser recipient,
            string resetPasswordLink)
        {
            var emailDetails = new SendEmailDetails
            {
                FromEmail = "accounts@modsdeal.com",
                ToEmail = recipient.Email,
                ToName = recipient.FirstName,
                Subject = "Password reset for ModsDeal",
            };

            return await templatedEmailSender
                .SendEmailAsync(emailDetails, "message",
                new()
                {
                    { "--Username--", recipient.UserName },
                    { "--Content1--", "A password reset request was made for this account." },
                    {
                        "--Content2--",
                        "If it was made by you, click the button below to reset the password " +
                        "otherwise, simply ignore this message."
                    },
                    { "--Link--", resetPasswordLink },
                    { "--LinkAction--", "Confirm" }
                });
        }
    }
}
