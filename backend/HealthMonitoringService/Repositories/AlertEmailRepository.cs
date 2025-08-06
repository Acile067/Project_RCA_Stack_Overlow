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

        public async Task UpdateEmailStatusAsync(string rowKey, bool status)
        {
            TableOperation retrieve = TableOperation.Retrieve<AlertEmailEntity>("Alert", rowKey);
            TableResult result = await _table.ExecuteAsync(retrieve);
            var entity = result.Result as AlertEmailEntity;

            if (entity != null)
            {
                entity.IsEmailReceived = status;
                entity.ETag = "*";
                var update = TableOperation.Replace(entity);
                await _table.ExecuteAsync(update);
            }
        }

        public async Task ResetAllEmailStatusesAsync()
        {
            var query = new TableQuery<AlertEmailEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Alert"));

            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);

            foreach (var entity in segment.Results)
            {
                entity.IsEmailReceived = false;
                entity.ETag = "*"; // overwrite without version check
                var operation = TableOperation.Replace(entity);
                await _table.ExecuteAsync(operation);
            }
        }

        public CloudTable GetTable() => _table;

    }
}
