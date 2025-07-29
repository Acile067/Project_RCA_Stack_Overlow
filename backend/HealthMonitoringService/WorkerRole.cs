using HealthMonitoringService.HelperMethods;
using HealthMonitoringService.Repositories;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private AlertEmailRepository alertRepo;
        private HealthCheckRepository checkRepo;
        private CloudQueue queue;

        public override bool OnStart()
        {
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 12;

            alertRepo = new AlertEmailRepository();
            checkRepo = new HealthCheckRepository();
            queue = QueueHelper.GetQueueReference("emails");

            // Reset IsEmailReceived value to false
            alertRepo.ResetAllEmailStatusesAsync().Wait();

            Trace.TraceInformation("HealthMonitoringService has been started");
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
            }
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");
            cancellationTokenSource.Cancel();
            runCompleteEvent.WaitOne();
            base.OnStop();
            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var stackOverflowCheck = HealthCheckHelper.CheckStackOverflowServiceAsync(checkRepo, alertRepo, queue);
                var notificationCheck = HealthCheckHelper.CheckNotificationServiceAsync(checkRepo, alertRepo, queue);

                await Task.WhenAll(stackOverflowCheck, notificationCheck);
                await Task.Delay(4000, token);
            }
        }
    }
}
