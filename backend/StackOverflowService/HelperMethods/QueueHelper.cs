using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StackOverflowService.HelperMethods
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
        public static async Task EnqueueTopAnswerIdAsync(string topAnswerId, CloudQueue queue)
        {
            if (string.IsNullOrWhiteSpace(topAnswerId))
                return;

            try
            {
                CloudQueueMessage message = new CloudQueueMessage(topAnswerId);
                await queue.AddMessageAsync(message);
                Trace.TraceInformation($"[QUEUE] Enqueued TopAnswerId: {topAnswerId}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[QUEUE] Failed to enqueue TopAnswerId: {ex.Message}");
            }
        }

    }
}