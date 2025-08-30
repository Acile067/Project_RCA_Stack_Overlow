using HealthStatusService.DTOs;
using HealthStatusService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HealthStatusService.Services
{
    public class HealthCheckService
    {
        private readonly HealthCheckRepository _healthCheckRepository;
        public HealthCheckService(HealthCheckRepository healthCheckRepository)
        {
            _healthCheckRepository = healthCheckRepository;
        }

        public async Task<HealthCheckSummaryDto> GetLast3hHealthChecksSummaryAsync()
        {
            var entities = await _healthCheckRepository.GetLast3hHealtChecksAsync("StackOverflowService");

            var checks = entities.Select(e => new HealthCheckDto
            {
                Status = e.Status,
                CheckedAt = e.CheckTime
            }).ToList();

            int total = checks.Count;
            int okCount = checks.Count(c => c.Status == "OK");
            int notOkCount = checks.Count(c => c.Status == "NOT_OK");

            double availability = total == 0 ? 0 : (okCount / (double)total) * 100.0;

            return new HealthCheckSummaryDto
            {
                //Checks = checks,
                Total = total,
                OkCount = okCount,
                NotOkCount = notOkCount,
                AvailabilityPercentage = availability
            };
        }
    }
}