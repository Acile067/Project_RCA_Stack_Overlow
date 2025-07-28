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
    public class AlertEmailRepository
    {
        private readonly CloudTable _table;
        private readonly CloudTableClient _tableClient;
        public AlertEmailRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("DataConnectionString"));

            _tableClient = storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference("AlertEmails");
            _table.CreateIfNotExistsAsync().Wait();
        }     

        public async Task<List<AlertEmailEntity>> GetAllEmailsAsync()
        {
            var query = new TableQuery<AlertEmailEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Alert"));

            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results;
        }
              
    }
}
