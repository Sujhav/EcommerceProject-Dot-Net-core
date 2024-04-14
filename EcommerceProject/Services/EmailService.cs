using EcommerceProject.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EcommerceProject.Services
{
    public class EmailService
    {
        private const string TemplatePath = @"EmailTemplate/{0}.html";

        private readonly SMTPConfigModel _SMTPConfig;

        public EmailService(IOptions<SMTPConfigModel> smtpconfig)
        {
            _SMTPConfig = smtpconfig.Value;
        }

        public async Task SendTestEmail(EmailUsers emailUsers)
        {
            emailUsers.Subject=UpdatePlaceHolders("hello {{username}},this is a text",emailUsers.PlaceHolders);
            emailUsers.Body = UpdatePlaceHolders(GetEmailBody("SendEmail"), emailUsers.PlaceHolders);
            await SendEmails(emailUsers);

        }
        private async Task SendEmails(EmailUsers emailUsers)
        {
            MailMessage mailMessage = new MailMessage
            {
                Subject = emailUsers.Subject,
                Body = emailUsers.Body,
                From = new MailAddress(_SMTPConfig.SenderAddress, _SMTPConfig.SenderDisplayName),
                IsBodyHtml = _SMTPConfig.IsBodyHtml,

            };
            foreach(var toEmails in  emailUsers.ToEmails)
            {
                mailMessage.To.Add(toEmails);
            }
            NetworkCredential networkCredential = new NetworkCredential (_SMTPConfig.UserName,_SMTPConfig.Password);
            SmtpClient smtpClient = new SmtpClient
            {
                Host = _SMTPConfig.host,
                Port = _SMTPConfig.port,
                EnableSsl = _SMTPConfig.EnableSSl,
                UseDefaultCredentials = _SMTPConfig.UseDefaultCredentials,
                Credentials = networkCredential,
            };
            mailMessage.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mailMessage);
        }


        private string GetEmailBody(string template)
        {
            var body=File.ReadAllText(string.Format(TemplatePath, template));
            return body;
        }

        private string UpdatePlaceHolders(string text,List<KeyValuePair<string,string>> keyValuePairs)
        {
            
            if(!string.IsNullOrEmpty(text))
            {
                foreach(var pair in keyValuePairs) 
                {
                    if (text.Contains(pair.Key))
                    {
                        text=text.Replace(pair.Key, pair.Value);
                    }
                }
            }
            return text;
        }

        public async Task SendEmailForConfirmation(EmailUsers emailUsers)
        {
            emailUsers.Subject=UpdatePlaceHolders("hello {{username}},this is email confirmation",emailUsers.PlaceHolders);
            emailUsers.Body = UpdatePlaceHolders(GetEmailBody("SendEmailConfirmation"), emailUsers.PlaceHolders);
            await SendEmails(emailUsers);
        }

        public async Task SendResetPassword(EmailUsers emailUsers)
        {
            emailUsers.Subject = UpdatePlaceHolders("hello {{username}},Reset Your Password", emailUsers.PlaceHolders);
            emailUsers.Body = UpdatePlaceHolders(GetEmailBody("PasswordReset"), emailUsers.PlaceHolders);
            await SendEmails(emailUsers);
        }

    }
}
