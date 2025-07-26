using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminToolsConsoleApp.Entities;
using Microsoft.Azure;

namespace AdminToolsConsoleApp.Repositories
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

        public async Task<List<string>> GetAllEmailsAsync()
        {
            var query = new TableQuery<AlertEmailEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Alert"));

            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results.Select(e => e.EmailAddress).ToList();
        }

        public async Task AddEmailAsync(string email)
        {
            var entity = new AlertEmailEntity(email);
            var insert = TableOperation.Insert(entity);
            await _table.ExecuteAsync(insert);
        }

        public async Task<bool> DeleteEmailAsync(string emailToDelete)
        {
            var query = new TableQuery<AlertEmailEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Alert"));

            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            var match = segment.Results.FirstOrDefault(e => e.EmailAddress == emailToDelete);

            if (match != null)
            {
                var delete = TableOperation.Delete(match);
                await _table.ExecuteAsync(delete);
                return true;
            }

            return false;
        }
    }
}
