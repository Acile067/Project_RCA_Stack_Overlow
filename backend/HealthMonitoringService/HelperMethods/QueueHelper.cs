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
using Microsoft.WindowsAzure.Storage.Table;

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
                int count = 0;

                foreach (var email in emails)
                {
                    if (email.IsEmailReceived)
                        continue;

                    email.IsEmailReceived = true;
                    email.ETag = "*";

                    try
                    {
                        var updateOp = TableOperation.Replace(email);
                        await alertRepo.GetTable().ExecuteAsync(updateOp);

                        var message = new CloudQueueMessage(email.EmailAddress);
                        await queue.AddMessageAsync(message);

                        count++;
                    }
                    catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 412)
                    {
                        Trace.TraceInformation($"[QUEUE] Email {email.EmailAddress} already handled by another instance.");
                    }
                }

                Trace.TraceInformation($"[QUEUE] Enqueued {count} email(s)");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[QUEUE] Failed to enqueue alert emails: {ex.Message}");
            }
        }


    }
}
