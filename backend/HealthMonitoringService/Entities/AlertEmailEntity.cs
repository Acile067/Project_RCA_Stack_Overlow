using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService.Entities
{
    public class AlertEmailEntity : TableEntity
    {
        public AlertEmailEntity() { }
        public string EmailAddress { get; set; }
        public bool IsEmailReceived { get; set; }
    }
}
