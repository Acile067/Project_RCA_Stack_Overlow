using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthStatusService.DTOs
{
    public class HealthCheckSummaryDto
    {
        //public List<HealthCheckDto> Checks { get; set; }
        public int Total { get; set; }
        public int OkCount { get; set; }
        public int NotOkCount { get; set; }
        public double AvailabilityPercentage { get; set; }
    }
}