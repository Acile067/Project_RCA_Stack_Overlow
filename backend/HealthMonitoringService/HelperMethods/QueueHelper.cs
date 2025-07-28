using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoringService.Repositories;
using HealthMonitoringService.Entities;
using System.Diagnostics;

namespace HealthMonitoringService.HelperMethods
{
    public class QueueHelper
    {
        public static CloudQueue GetQueueReference(String queueName)
        {
            CloudStorageAccount storageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
        }

        public static async Task EnqueueAlertEmailsAsync(AlertEmailRepository alertRepo, CloudQueue queue)
        {
            try
            {
                List<AlertEmailEntity> emails = await alertRepo.GetAllEmailsAsync();

                foreach (var email in emails)
                {
                    CloudQueueMessage message = new CloudQueueMessage(email.EmailAddress);
                    await queue.AddMessageAsync(message);
                }

                Trace.TraceInformation($"[QUEUE] Enqueued {emails.Count} email(s)");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[QUEUE] Failed to enqueue alert emails: {ex.Message}");
            }
        }
    }
}
