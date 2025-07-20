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
    public class QuestionRepository
    {
        private readonly CloudTable _table;

        public QuestionRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var client = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            _table = client.GetTableReference("Question");
            _table.CreateIfNotExists();
        }

        public async Task SaveQuestionAsync(Question question)
        {
            var insert = TableOperation.Insert(new QuestionTableEntity(question));
            await _table.ExecuteAsync(insert);
        }
        public async Task<List<QuestionTableEntity>> GetAllQuestionsAsync()
        {
            var query = new TableQuery<QuestionTableEntity>();
            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results;
        }

        public async Task<List<QuestionTableEntity>> GetQuestionsByDateRangeAsync(DateTime from, DateTime to)
        {
            string fromFilter = TableQuery.GenerateFilterConditionForDate("CreatedAt", QueryComparisons.GreaterThanOrEqual, from);
            string toFilter = TableQuery.GenerateFilterConditionForDate("CreatedAt", QueryComparisons.LessThanOrEqual, to);
            string combinedFilter = TableQuery.CombineFilters(fromFilter, TableOperators.And, toFilter);

            var query = new TableQuery<QuestionTableEntity>().Where(combinedFilter);
            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results;
        }

        public async Task CloseQuestionAsync(string questionId, string topAnswerId)
        {
            var question = await GetQuestionByIdAsync(questionId);
            if (question != null)
            {
                question.IsClosed = true;
                question.TopAnswerId = topAnswerId;
                question.UpdatedAt = DateTime.UtcNow;

                var operation = TableOperation.Replace(new QuestionTableEntity
                {
                    PartitionKey = question.PartitionKey,
                    RowKey = question.RowKey,
                    Title = question.Title,
                    Description = question.Description,
                    CreatedBy = question.CreatedBy,
                    TopAnswerId = question.TopAnswerId,
                    IsClosed = question.IsClosed,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    ETag = "*"
                });

                await _table.ExecuteAsync(operation);
            }
        }

        public async Task<QuestionTableEntity> GetQuestionByIdAsync(string id)
        {
            var query = new TableQuery<QuestionTableEntity>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));

            var segment = await _table.ExecuteQuerySegmentedAsync(query, null);
            return segment.Results.FirstOrDefault();
        }
    }
}