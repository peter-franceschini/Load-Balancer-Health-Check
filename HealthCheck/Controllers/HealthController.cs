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
        private Settings Settings { get; set; }
        private IHealthService HealthService { get; set; }

        public HealthController(Settings settings, IHealthService healthService)
        {
            Settings = settings;
            HealthService = healthService;
        }
    }
}
