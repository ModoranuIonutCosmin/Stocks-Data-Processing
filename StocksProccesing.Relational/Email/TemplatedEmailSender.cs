using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StocksFinalSolution.BusinessLogic.Interfaces.Email;
using StocksProccesing.Relational.Email.Dtos;

namespace StocksProccesing.Relational.Email
{
    public class TemplatedEmailSender : ITemplatedEmailSender
    {
        private readonly IDirectEmailSender directEmailSender;

        public TemplatedEmailSender(IDirectEmailSender directEmailSender,
            ILogger<TemplatedEmailSender> logger)
        {
            this.directEmailSender = directEmailSender;
        }
        public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails,
            string templateName, Dictionary<string, string> substitutions)
        {
            string templatePath = Path.Combine(".",
                "Email", "Template",
                $"{templateName}.html");


            var content = default(string);
            using (var reader = new StreamReader(templatePath))
            {
                content = await reader.ReadToEndAsync();
            }

            foreach (var substitution in substitutions)
            {
                content = content.Replace(substitution.Key, substitution.Value);
            }

            sendEmailDetails.Content = content;

            sendEmailDetails.IsHTML = true;

            return await directEmailSender.SendEmailAsync(sendEmailDetails);
        }
    }
}
