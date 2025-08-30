using HealthStatusService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HealthStatusService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HealthCheckController : ApiController
    {
        private readonly HealthCheckService _healthCheckService;
        public HealthCheckController()
        {
            var healthCheckRepo = new Repositories.HealthCheckRepository();
            _healthCheckService = new HealthCheckService(healthCheckRepo);
        }
        [HttpGet]
        [Route("api/healthchecks/summary")]
        public async Task<IHttpActionResult> GetHealthCheckSummaryAsync()
        {
            var summary = await _healthCheckService.GetLast3hHealthChecksSummaryAsync();
            return Ok(summary);
        }
    }
}
