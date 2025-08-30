using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthStatusService.AzureStorage
{
    public class HealthCheckTableEntity : TableEntity
    {
        public HealthCheckTableEntity() { }
        public HealthCheckTableEntity(string serviceName)
        {
            PartitionKey = "HealthCheck";
            RowKey = Guid.NewGuid().ToString();
            ServiceName = serviceName;
            CheckTime = DateTime.UtcNow;
        }
        public string ServiceName { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; }
    }
}