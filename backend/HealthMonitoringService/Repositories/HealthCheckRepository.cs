using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoringService.Entities;

namespace HealthMonitoringService.Repositories
{
    public class HealthCheckRepository
    {

        private readonly CloudTable _table;
        private readonly CloudTableClient _tableClient;
        public HealthCheckRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("DataConnectionString"));

            _tableClient = storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference("HealthCheck");
            _table.CreateIfNotExistsAsync().Wait();
        }

        public async Task SaveHealthCheckAsync(string serviceName, bool isOk)
        {
            var entity = new HealthCheckEntity(serviceName)
            {
                Status = isOk ? "OK" : "NOT_OK"
            };

            var insert = TableOperation.Insert(entity);
            await _table.ExecuteAsync(insert);
        }
    }
}
