using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StackOverflow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Infrastructure.Common
{
    public class UserTableContext
    {
        public CloudStorageAccount _storageAccount;
        public CloudTable _table;

        public UserTableContext()
        {
            _storageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable");
            _table.CreateIfNotExists();
        }

        public async Task<bool> InsertOrUpdateEntityAsync(User user)
        {
            if (user == null)
            {
                return false;
            }
            try
            {
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(user);
                await _table.ExecuteAsync(insertOrReplaceOperation);
                return true;
            }
            catch (StorageException ex)
            {
                Console.WriteLine("Azure Table insert error: " + ex.Message);
                // Log the exception (not implemented here)
                return false;
            }
        }

        public async Task<bool> IsEmailExistingAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            try
            {
                TableQuery<User> query = new TableQuery<User>().Where(
                    TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));
                var result = await _table.ExecuteQuerySegmentedAsync(query, null);
                return result.Results.Any();
            }
            catch (StorageException ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }

        public async Task<Domain.Entities.User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            try
            {
                TableQuery<Domain.Entities.User> query = new TableQuery<Domain.Entities.User>().Where(
                    TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

                var result = await _table.ExecuteQuerySegmentedAsync(query, null);
                return result.Results.FirstOrDefault();
            }
            catch (StorageException)
            {
                return null;
            }
        }
        public async Task<Domain.Entities.User> GetUserByIdAsync(string userId)
        {
            
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Domain.Entities.User>("User", userId);
                var result = await _table.ExecuteAsync(retrieveOperation);
                return result.Result as Domain.Entities.User;
            }
            catch (StorageException)
            {
                return null;
            }
        }
    }
}
