using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class HealthCheckMailService
    {
        public static async Task SendEmailsFromQueueAsync(CloudQueue queue)
        {
            try
            {
                CloudQueueMessage message;

                while ((message = await queue.GetMessageAsync()) != null)
                {
                    string emailAddress = message.AsString;

                    bool success = SendEmail(emailAddress);

                    if (success)
                    {
                        await queue.DeleteMessageAsync(message);
                        Trace.TraceInformation($"[EMAIL] Sent and removed message for {emailAddress}");
                    }
                    else
                    {
                        Trace.TraceWarning($"[EMAIL] Failed to send email to {emailAddress}, message left in queue");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[QUEUE] Error while sending emails from queue: {ex.Message}");
            }
        }

        private static bool SendEmail(string toEmail)
        {
            try
            {
                // Read values from environment variables
                string smtpUsername = Environment.GetEnvironmentVariable("MAIL_USERNAME");
                string smtpPassword = Environment.GetEnvironmentVariable("MAIL_PASSWORD");
                string smtpServer = Environment.GetEnvironmentVariable("MAIL_SERVER") ?? "smtp.gmail.com";
                int smtpPort = int.Parse(Environment.GetEnvironmentVariable("MAIL_PORT") ?? "587");
                bool enableSsl = bool.Parse(Environment.GetEnvironmentVariable("MAIL_USE_TLS") ?? "true");

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = enableSsl
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "Service Down Alert",
                    Body = "One of the monitored services is not responding.",
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[SMTP] Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

    }
}
