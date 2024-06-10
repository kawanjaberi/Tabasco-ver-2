using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tabasco.Services;

namespace Tabasco.Controllers
{
	[ApiController]
	[Route("health")]
	public class HealthController : ControllerBase
	{
		private readonly ILogger<HealthController> _logger;
		private readonly IHealthCheckService _healthCheckService;
		
		public HealthController(IHealthCheckService healthCheckService, ILogger<HealthController> logger)
		{
			_healthCheckService	=	healthCheckService;
			_logger 			= 	logger;
		}
		
		[HttpGet("liveness")]
		public IActionResult Liveness()
		{
			return Ok(new {	message = "Healthy"	});
		}
		
		[HttpGet("readiness")]
		public async Task<IActionResult> Readiness()
		{
			var kavenegarHealth =	await _healthCheckService.CheckKavenegarHealthAsync();
			// var status			=	kavenegarHealth ? "Up" : "Down";
			var status = kavenegarHealth.Status;
			
			return Ok(new 
			{
				message	= "readiness",
				data 	= new
				{
					status	= "Healthy",
					info 	= new {kavenegar = new {status}},
					error	= new {},
					details = new {kavenegar = new {status}}
				}
			});
		}
	}
}