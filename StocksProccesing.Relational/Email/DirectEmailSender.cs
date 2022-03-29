using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stocks.General.Models;
using StocksFinalSolution.BusinessLogic.Interfaces.Email;

namespace StocksProccesing.Relational.Email;

public class DirectEmailSender : IDirectEmailSender
{
    private readonly IConfiguration _configuration;

    public DirectEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    ///     Trimite un email folosind server SMTP
    /// </summary>
    /// <param name="sendEmailDetails"> Obiect continand parametrii despre email-ul de trimis</param>
    /// <returns>Raspuns indicand succes-ul operatiunii</returns>
    public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails sendEmailDetails)
    {
        var errorMessage = "Invalid email sending parameters!";

        //Verifica daca detaliile sunt valide
        if (string.IsNullOrWhiteSpace(sendEmailDetails.ToEmail) ||
            string.IsNullOrWhiteSpace(sendEmailDetails.FromEmail) ||
            string.IsNullOrWhiteSpace(sendEmailDetails.Subject) ||
            string.IsNullOrWhiteSpace(sendEmailDetails.Content)
           )
            return new SendEmailResponse
            {
                ErrorMessage = errorMessage
            };

        //Creeaza mesajul si adauga-i continutul specificat in sendEmailDetails
        MailMessage mailMessage = new()
        {
            From = new MailAddress(sendEmailDetails.FromEmail),
            Subject = sendEmailDetails.Subject,
            IsBodyHtml = sendEmailDetails.IsHTML,
            Body = sendEmailDetails.Content
        };
        mailMessage.To.Add(new MailAddress(sendEmailDetails.ToEmail));

        //Initializeaza datele despre serverul smtp care trimite mail-ul
        using var smtpClient = new SmtpClient();

        smtpClient.EnableSsl = true;
        smtpClient.Host = _configuration["Smtp:Host"];
        smtpClient.Credentials = new NetworkCredential(_configuration["Smtp:Email"],
            _configuration["Smtp:Password"]);
        smtpClient.Port = int.Parse(_configuration["Smtp:Port"]);
        smtpClient.UseDefaultCredentials = false;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        try
        {
            //Trimite mail-ul
            await Task.Run(() => smtpClient.Send(mailMessage));
        }
        catch (Exception ex)
        {
            //Trimite mai departe mesajul execeptiei daca trimiterea email-ului
            //esueaza
            return new SendEmailResponse
            {
                ErrorMessage = ex.Message
            };
        }

        return new SendEmailResponse();
    }
}