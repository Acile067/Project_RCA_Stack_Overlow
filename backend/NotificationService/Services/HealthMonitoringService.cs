using NotificationService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class HealthMonitoringService : IHealthMonitoring
    {
        public string HealthCheck()
        {
            return "I am alive";
        }
    }
}
