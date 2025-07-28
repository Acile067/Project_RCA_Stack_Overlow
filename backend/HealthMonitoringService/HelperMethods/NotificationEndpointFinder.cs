using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService.HelperMethods
{
    public class NotificationEndpointFinder
    {
        public static string GetHealthMonitoringEndpoint()
        {
            var role = RoleEnvironment.Roles["NotificationService"];
            var instance = role.Instances.First(); // ili prođi kroz sve ako želiš failover/load balancing
            var endpoint = instance.InstanceEndpoints["HealthMonitoring"];

            return $"net.tcp://{endpoint.IPEndpoint}/HealthMonitoring";
        }
    }
}
