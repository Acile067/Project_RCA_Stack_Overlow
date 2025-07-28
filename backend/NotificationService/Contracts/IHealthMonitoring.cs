using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace NotificationService.Contracts
{
    [ServiceContract]
    public interface IHealthMonitoring
    {

        [OperationContract]
        string HealthCheck();
    }
}
