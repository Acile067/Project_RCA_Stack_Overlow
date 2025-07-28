using HealthMonitoringService.Entities;
using HealthMonitoringService.HelperMethods;
using HealthMonitoringService.Repositories;
using Microsoft.WindowsAzure.Storage.Queue;
using NotificationService.Contracts;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HealthMonitoringService.HelperMethods
{
    public class HealthCheckHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task CheckStackOverflowServiceAsync(HealthCheckRepository checkRepo, AlertEmailRepository alertRepo, CloudQueue queue)
        {
            string serviceName = "StackOverflowService";
            string url = "http://localhost:5050/HealthMonitoring";

            bool isOk = await PingHttpServiceAsync(url);
            await checkRepo.SaveHealthCheckAsync(serviceName, isOk);

            if (!isOk)
                await QueueHelper.EnqueueAlertEmailsAsync(alertRepo, queue);
        }

        public static async Task CheckNotificationServiceAsync(HealthCheckRepository checkRepo, AlertEmailRepository alertRepo, CloudQueue queue)
        {
            string serviceName = "NotificationService";
            string endpoint = "net.tcp://localhost:10100/HealthMonitoring";

            bool isOk = PingWcfService(endpoint);
            await checkRepo.SaveHealthCheckAsync(serviceName, isOk);

            if (!isOk)
                await QueueHelper.EnqueueAlertEmailsAsync(alertRepo, queue);
        }

        private static async Task<bool> PingHttpServiceAsync(string url)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result.Trim('"') == "I am alive";
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[HTTP Ping] Error contacting {url}: {ex.Message}");
                return false;
            }
        }

        private static bool PingWcfService(string endpoint)
        {
            try
            {
                var binding = new NetTcpBinding();
                var factory = new ChannelFactory<IHealthMonitoring>(binding, new EndpointAddress(endpoint));
                var proxy = factory.CreateChannel();

                string result = proxy.HealthCheck();
                ((ICommunicationObject)proxy).Close();

                return result == "I am alive";
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[WCF Ping] Error contacting {endpoint}: {ex.Message}");
                return false;
            }
        }
    }
}
