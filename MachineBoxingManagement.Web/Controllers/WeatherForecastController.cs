using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Services;

namespace MachineBoxingManagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CAEDB01Context _context;
        private readonly BoxInService _boxinService;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CAEDB01Context context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _configuration = config;

            _boxinService = new BoxInService(
                _configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"),
                _configuration.GetValue<string>("ConnectionStrings:CAEServiceConnection")
                );
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
    
            var qq = _context.BoxingLocation.ToList();
            var qqq = _boxinService.GetBoxingName("", "", "", "", DateTime.Now);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
