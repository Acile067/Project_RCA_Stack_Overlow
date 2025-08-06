using Microsoft.WindowsAzure.Storage.Table;
using NotificationService.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Entities;
using Microsoft.WindowsAzure.Storage.Queue;
using NotificationService.Repositories;
using System.Diagnostics;

namespace NotificationService.Services
{
    public class CloseQuestionMailService
    {
        private static CloudQueue queue;
        public static async Task HandleTopAnswerAsync(string topAnswerId)
        {
            // 1. Nađi Answer po RowKey = TopAnswerId
            var answerTable = TableHelper.GetTable("Answer");
            var answerQuery = new TableQuery<AnswerTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, topAnswerId));

            var answerSegment = await answerTable.ExecuteQuerySegmentedAsync(answerQuery, null);
            var answer = answerSegment.Results.FirstOrDefault();
            if (answer == null) return;

            string questionId = answer.QuestionId;

            // 2. Nađi pitanje po questionId
            var questionTable = TableHelper.GetTable("Question");
            var questionQuery = new TableQuery<QuestionTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, questionId));

            var questionSegment = await questionTable.ExecuteQuerySegmentedAsync(questionQuery, null);
            var question = questionSegment.Results.FirstOrDefault();
            if (question == null) return;

            string creatorEmail = question.CreatedBy;

            // 3. Nađi korisničko ime kreatora
            var userTable = TableHelper.GetTable("Users");
            var userQuery = new TableQuery<UserTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, creatorEmail));

            var userSegment = await userTable.ExecuteQuerySegmentedAsync(userQuery, null);
            var user = userSegment.Results.FirstOrDefault();
            string username = user?.FullName ?? "Unknown user";

            // 4. Nađi sve email adrese koje su ostavile odgovor na ovo pitanje
            string answerPartitionKey = $"Answer_{questionId}";
            var allAnswersQuery = new TableQuery<AnswerTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, answerPartitionKey));

            var allAnswerSegment = await answerTable.ExecuteQuerySegmentedAsync(allAnswersQuery, null);
            var recipientEmails = allAnswerSegment.Results
                .Select(a => a.AnsweredByEmail)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct();

            // 5. Pošalji mejl svakom
            foreach (var email in recipientEmails)
            {
                await SendEmailAsync(email,
                    subject: "Question closed",
                    body: $"Question you left an answer for has been closed by user: {username}.\n" +
                    $"Best answer is:\n{answer.Description}");
                Trace.TraceInformation($"[HandleTopAnswerAsync] Email sent to {email}");
            }

            try
            {
                var notificationRepo = new NotificationsRepository();
                await notificationRepo.SaveNotificationAsync(topAnswerId, recipientEmails.Count());
            }
            catch (Exception ex)
            {
                Trace.TraceError("[NotificationInsert] Failed: " + ex.Message);
            }
        }
        public static async Task SendEmailAsync(string to, string subject, string body)
        {
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
                Subject = subject,
                Body = body,                
                IsBodyHtml = false
            };

            mail.To.Add(to);
            await smtpClient.SendMailAsync(mail);
        }

        public static async Task ProcessTopAnswerQueueAsync()
        {
            queue = QueueHelper.GetQueueReference("top-answer-notification");

            while (true)
            {
                var message = await queue.GetMessageAsync();
                if (message == null) break;

                string topAnswerId = message.AsString;

                await HandleTopAnswerAsync(topAnswerId);

                await queue.DeleteMessageAsync(message); 
            }
        }

    }
}
