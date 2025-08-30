using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthStatusService.Entities
{
    public class HealthCheck
    {
        public string ServiceName { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; }
    }
}