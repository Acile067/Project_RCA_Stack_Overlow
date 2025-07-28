using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace StackOverflowService.Controllers
{
    public class HealthMonitoringController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("HealthMonitoring")]
        public async Task<IHttpActionResult> HealthCheck()
        {
            return Ok("I am alive");
        }

    }
}
