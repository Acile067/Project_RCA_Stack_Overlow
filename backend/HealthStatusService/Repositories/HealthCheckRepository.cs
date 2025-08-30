using HealthStatusService.AzureStorage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HealthStatusService.Repositories
{
    public class HealthCheckRepository
    {
        private readonly CloudTable _table;

        public HealthCheckRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var client = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            _table = client.GetTableReference("HealthCheck");
            _table.CreateIfNotExists();
        }

        public async Task<List<HealthCheckTableEntity>> GetLast3hHealtChecksAsync(string serviceName)
        {
            var results = new List<HealthCheckTableEntity>();

            string timeFilter = TableQuery.GenerateFilterConditionForDate(
                nameof(HealthCheckTableEntity.CheckTime),
                QueryComparisons.GreaterThanOrEqual,
                DateTimeOffset.UtcNow.AddHours(-3));

            string partitionFilter = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                "Check");

            string serviceFilter = TableQuery.GenerateFilterCondition(
                nameof(HealthCheckTableEntity.ServiceName),
                QueryComparisons.Equal,
                serviceName);

            string combinedFilter = TableQuery.CombineFilters(
                TableQuery.CombineFilters(partitionFilter, TableOperators.And, timeFilter),
                TableOperators.And,
                serviceFilter);

            var query = new TableQuery<HealthCheckTableEntity>().Where(combinedFilter);

            TableContinuationToken token = null;
            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(segment.Results);
                token = segment.ContinuationToken;
            }
            while (token != null);

            return results;
        }
    }
}