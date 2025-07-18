using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StackOverflowService.AzureStorage;
using StackOverflowService.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StackOverflowService.Repositories
{
    public class UserRepository
    {
        private readonly CloudTable _table;

        public UserRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var client = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            _table = client.GetTableReference("Users");
            _table.CreateIfNotExists();
        }

        public async Task SaveUserAsync(User user)
        {
            var insert = TableOperation.Insert(new UserTableEntity(user));
            await _table.ExecuteAsync(insert);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            var query = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email)
            );

            var token = new TableContinuationToken();
            var segment = await _table.ExecuteQuerySegmentedAsync(query, token);

            return segment.Results.Any(); // true ako postoji korisnik sa tim emailom
        }
        public async Task<UserTableEntity> GetByEmailAsync(string email)
        {
            var query = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email)
            );

            var result = await _table.ExecuteQuerySegmentedAsync(query, null);
            return result.Results.FirstOrDefault();
        }
    }
}