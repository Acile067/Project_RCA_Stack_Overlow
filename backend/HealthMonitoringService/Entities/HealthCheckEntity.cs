using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService.Entities
{
    public class HealthCheckEntity : TableEntity
    {
        public HealthCheckEntity() { }

        public HealthCheckEntity(string serviceName)
        {
            PartitionKey = "Check";
            RowKey = Guid.NewGuid().ToString(); 
            ServiceName = serviceName;
            CheckTime = DateTime.UtcNow;
        }

        public string ServiceName { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; } 
    }
}
