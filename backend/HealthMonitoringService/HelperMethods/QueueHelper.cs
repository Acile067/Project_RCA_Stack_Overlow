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
                    // 🔒 Provera da li je već enqueued
                    if (email.IsEmailReceived)
                        continue;

                    // Postavi flag na true pre nego što enqueue-uješ
                    email.IsEmailReceived = true;
                    email.ETag = "*"; // force update

                    try
                    {
                        // ⚠ Ažuriraj status u tabeli — ako druga instanca pokuša isto, samo jedna će proći
                        var updateOp = TableOperation.Replace(email);
                        await alertRepo.GetTable().ExecuteAsync(updateOp); // expose GetTable() from repository

                        // ✅ Tek sada dodaj poruku u queue
                        var message = new CloudQueueMessage(email.EmailAddress);
                        await queue.AddMessageAsync(message);

                        count++;
                    }
                    catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 412)
                    {
                        // ❌ Druga instanca je već ažurirala - ignorisi
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
