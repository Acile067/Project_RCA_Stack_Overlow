using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StackOverflowService.AzureStorage;
using StackOverflowService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StackOverflowService.Repositories
{
    public class AnswerRepository
    {
        private readonly CloudTable _table;

        public AnswerRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var client = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            _table = client.GetTableReference("Answer");
            _table.CreateIfNotExists();
        }

        public async Task SaveAnswerAsync(Answer answer)
        {
            answer.Id = Guid.NewGuid().ToString();
            answer.CreatedAt = DateTime.UtcNow;

            var entity = new AnswerTableEntity(answer);
            var insert = TableOperation.Insert(entity);
            await _table.ExecuteAsync(insert);
        }

        public async Task<List<AnswerTableEntity>> GetAnswersByQuestionIdAsync(string questionId)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, $"Answer_{questionId}");
            var query = new TableQuery<AnswerTableEntity>().Where(filter);
            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results;
        }
    }
}