using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp.Entities
{
    public class AlertEmailEntity : TableEntity
    {
        public AlertEmailEntity() { }

        public AlertEmailEntity(string email)
        {
            PartitionKey = "Alert";
            RowKey = Guid.NewGuid().ToString();
            EmailAddress = email;
        }

        public string EmailAddress { get; set; }
    }
}
