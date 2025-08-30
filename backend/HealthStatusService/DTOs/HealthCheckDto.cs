using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthStatusService.DTOs
{
    public class HealthCheckDto
    {
        public string Status { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}