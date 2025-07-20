using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StackOverflowService.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StackOverflowService.Repositories
{
    public class VoteRepository
    {
        private readonly CloudTable _table;

        public VoteRepository()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            var client = new CloudTableClient(new Uri(storageAccount.TableEndpoint.AbsoluteUri), storageAccount.Credentials);
            _table = client.GetTableReference("Vote");
            _table.CreateIfNotExists();
        }

        public async Task<bool> HasUserAlreadyVotedAsync(string answerId, string userEmail)
        {
            var retrieve = TableOperation.Retrieve<VoteTableEntity>($"Vote_{answerId}", userEmail);
            var result = await _table.ExecuteAsync(retrieve);
            return result.Result != null;
        }

        public async Task SaveVoteAsync(string answerId, string userEmail)
        {
            if (await HasUserAlreadyVotedAsync(answerId, userEmail))
                return;

            var vote = new VoteTableEntity(answerId, userEmail);
            var insert = TableOperation.Insert(vote);
            await _table.ExecuteAsync(insert);
        }
    }
}