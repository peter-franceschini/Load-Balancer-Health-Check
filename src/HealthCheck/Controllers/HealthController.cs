using HealthCheck.Models;
using HealthCheck.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private IHealthService HealthService { get; set; }

        public HealthController(IHealthService healthService)
        {
            HealthService = healthService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            if (HealthService.IsHealthy())
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
