using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kavenegar;
using Kavenegar.Exceptions;
using Tabasco.Models;

namespace Tabasco.Services
{
	public class HealthCheckService : IHealthCheckService
	{
		private readonly ILogger<HealthCheckService> _logger;
		private readonly KavenegarApi _api;
		
		// If need I will use below sender  number
		// private readonly string senderNumber = Environment.GetEnvironmentVariable("KAVEHNEGAR_SENDER_NUMBER");
		
		public HealthCheckService(string apiKey, ILogger<HealthCheckService> logger)
		{
			_api	= new KavenegarApi(apiKey);
			_logger = logger;
		}
		
		public async Task<KavenegarHealthCheckResult> CheckKavenegarHealthAsync()
		{
			try
			{
				// Simple health check by sending a minimal request	
				var accountInfoResult = await Task.Run(() => _api.AccountInfo());
				
				return new KavenegarHealthCheckResult
				{					
					Status	 	 = "Up",
					RemainCredit = accountInfoResult.RemainCredit
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "HTTP error occurred while checking Kavenegar health check");
				// throw;
				// return false;
				return new KavenegarHealthCheckResult
				{
					Status	= "Down",
					RemainCredit = 0
				};
				
			}
		}
	}
}