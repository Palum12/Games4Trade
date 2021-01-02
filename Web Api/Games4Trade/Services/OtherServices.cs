using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4TradeAPI.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MimeKit;

namespace Games4TradeAPI.Services
{
    public static class OtherServices
    {
 
        /// <summary>
        ///  Method used for returning detailed model error from validating.
        /// </summary>
        /// <param name="modelState"> State of validated model.</param>
        /// <returns> Collection of error messages.</returns>
        public static IEnumerable<string> ReturnAllModelErrors(ModelStateDictionary modelState)
        {
            var allErrors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return allErrors;
        }

        public static async Task<bool> SendEmail(string address, string title, string text)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("sender", "noreply@games4trade.pl"));
            message.To.Add(new MailboxAddress("reciever", address));
            message.Subject = title;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = string.Format(text)
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("", 465, true);

                    // Authenticate email with server
                    await client.AuthenticateAsync("", "");
                    // Send message to receiver
                    await client.SendAsync(message);
                    // Disconect to unlock unnecessary resources
                    await client.DisconnectAsync(true);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static OperationResult GetIncorrectDatabaseConnectionResult()
        {
            return new OperationResult()
            {
                IsSuccessful = false,
                IsClientError = false,
                Message = "Something went wrong with db connection!"
            };
        }
    }
}
